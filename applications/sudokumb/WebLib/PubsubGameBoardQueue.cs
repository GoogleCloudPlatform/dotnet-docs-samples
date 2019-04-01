// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sudokumb
{
    public class PubsubGameBoardQueueOptions
    {
        /// <summary>
        /// The Google Cloud project id.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// The Pub/sub subscription from which solve messages are read.
        /// </summary>
        public string SubscriptionId { get; set; } = "sudokumb4";

        /// <summary>
        /// The Pub/sub topic where solve messages are written.
        /// </summary>
        public string TopicId { get; set; } = "sudokumb4";

        /// <summary>
        /// When exploring the game tree, the number of branches that should
        /// be explored in parallel.
        /// </summary>
        /// <returns></returns>
        public int MaxParallelBranches { get; set; } = 20;
    }

    public static class PubsubGameBoardQueueExtensions
    {
        public static IServiceCollection AddPubsubGameBoardQueueSolver(
            this IServiceCollection services)
        {
            services.AddSingleton<PubsubGameBoardQueueSolver>();
            services.AddSingleton<IGameBoardQueue, PubsubGameBoardQueueSolver>(
                provider => provider.GetService<PubsubGameBoardQueueSolver>()
            );
            services.AddSingleton<IHostedService, PubsubGameBoardQueueSolver>(
                provider => provider.GetService<PubsubGameBoardQueueSolver>()
            );
            return services;
        }
    }

    public class PubsubGameBoardQueue : IGameBoardQueue
    {
        public readonly PublisherServiceApiClient _publisherApi;
        public readonly PublisherClient _publisherClient;
        public readonly SubscriberClient _subscriberClient;
        public readonly IOptions<PubsubGameBoardQueueOptions> _options;
        public readonly ILogger _logger;

        public PubsubGameBoardQueue(
            IOptions<PubsubGameBoardQueueOptions> options,
            ILogger<PubsubGameBoardQueue> logger)
        {
            _options = options;
            _logger = logger;
            _publisherApi = PublisherServiceApiClient.Create();
            var subscriberApi = SubscriberServiceApiClient.Create();
            _publisherClient = PublisherClient.CreateAsync(MyTopic).Result;
            _subscriberClient = SubscriberClient.CreateAsync(MySubscription,
                settings: new SubscriberClient.Settings()
                {
                    AckDeadline = TimeSpan.FromMinutes(1)
                }).Result;

            // Create the Topic and Subscription.
            try
            {
                _publisherApi.CreateTopic(MyTopic);
                _logger.LogInformation("Created {0}.", MyTopic.ToString());
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
            }

            try
            {
                subscriberApi.CreateSubscription(MySubscription, MyTopic,
                    pushConfig: null, ackDeadlineSeconds: 10);
                _logger.LogInformation("Created {0}.",
                    MySubscription.ToString());
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
            }
        }

        public TopicName MyTopic
        {
            get
            {
                var opts = _options.Value;
                return new TopicName(opts.ProjectId, opts.TopicId);
            }
        }

        public SubscriptionName MySubscription
        {
            get
            {
                var opts = _options.Value;
                return new SubscriptionName(opts.ProjectId,
                   opts.SubscriptionId);
            }
        }

        public async Task<bool> Publish(string solveRequestId,
            IEnumerable<GameBoard> gameBoards,
            CancellationToken cancellationToken)
        {
            var messages = gameBoards.Select(board => new GameBoardMessage()
            {
                SolveRequestId = solveRequestId,
                Stack = new[] { new BoardAndWidth { Board = board, ParallelBranches = 1 } },
            });
            var pubsubMessages = messages.Select(message => new PubsubMessage()
            {
                Data = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(
                    message))
            });
            await _publisherApi.PublishAsync(MyTopic, pubsubMessages,
                CallSettings.FromCancellationToken(cancellationToken));
            return false;
        }
    }

    // Implements a GameBoardQueue using Pub/sub.  The next boards to be
    // evaluated are published to a Pub/sub topic.
    public class PubsubGameBoardQueueSolver : PubsubGameBoardQueue, IHostedService
    {
        private readonly SolveStateStore _solveStateStore;
        private readonly IDumb _idumb;
        private readonly InMemoryGameBoardStackImpl _inMemoryGameBoardStack;
        private readonly Solver _solver;

        public PubsubGameBoardQueueSolver(
            IOptions<PubsubGameBoardQueueOptions> options,
            ILogger<PubsubGameBoardQueueSolver> logger,
            SolveStateStore solveStateStore, IDumb idumb,
            InMemoryGameBoardStackImpl inMemoryGameBoardStack,
            Solver solver) : base(options, logger)
        {
            _solveStateStore = solveStateStore;
            _idumb = idumb;
            _inMemoryGameBoardStack = inMemoryGameBoardStack;
            _solver = solver;
        }

        /// <summary>
        /// Solve one sudoku puzzle.
        /// </summary>
        /// <param name="pubsubMessage">The message as it arrived from Pub/Sub.
        /// </param>
        /// <returns>Ack or Nack</returns>
        private async Task<SubscriberClient.Reply> ProcessOneMessage(
            PubsubMessage pubsubMessage, CancellationToken cancellationToken)
        {
            // Unpack the pubsub message.
            string text = pubsubMessage.Data.ToString(Encoding.UTF8);
            GameBoardMessage message;
            try
            {
                message = JsonConvert.DeserializeObject<GameBoardMessage>(text);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Bad message in subscription {0}\n{1}",
                    MySubscription, text);
                return SubscriberClient.Reply.Ack;
            }
            if (message.Stack == null || message.Stack.Length == 0 ||
                string.IsNullOrEmpty(message.SolveRequestId))
            {
                _logger.LogError("Bad message in subscription {0}\n{1}",
                    MySubscription, text);
                return SubscriberClient.Reply.Ack;
            }
            if (null != (await _solveStateStore.GetCachedAsync(
                message.SolveRequestId, cancellationToken))?.Solution)
            {
                // Already solved.
                return SubscriberClient.Reply.Ack;
            }
            if (!await _idumb.IsDumbAsync())
            {
                // Solve the puzzle the smart way.
                await _inMemoryGameBoardStack.Publish(message.SolveRequestId,
                    message.Stack.Select(x => x.Board), cancellationToken);
                return SubscriberClient.Reply.Ack;
            }
            // Solve the puzzle the dumb way.
            // Examine the board.
            IEnumerable<GameBoard> nextMoves;
            _solveStateStore.IncreaseExaminedBoardCount(
                message.SolveRequestId, 1);
            BoardAndWidth top = message.Stack.Last();
            if (_solver.ExamineGameBoard(top.Board, out nextMoves))
            {
                // Yay!  Solved the game.
                _logger.LogInformation($"Solution: {top.Board.Board}");
                await _solveStateStore.SetAsync(message.SolveRequestId,
                    top.Board, cancellationToken);
                return SubscriberClient.Reply.Ack;
            }
            // Explore the next possible moves.
            List<Task> tasks = new List<Task>();
            List<GameBoard> stackMoves = new List<GameBoard>();
            int parallelBranches = top.ParallelBranches.GetValueOrDefault(
                _options.Value.MaxParallelBranches);
            int nextLevelWidth = (1 + nextMoves.Count()) * parallelBranches;
            if (nextLevelWidth > _options.Value.MaxParallelBranches)
            {
                // Too many branches already.  Explore this branch linearly.
                List<BoardAndWidth> stack =
                    new List<BoardAndWidth>(message.Stack.SkipLast(1));
                stack.AddRange(nextMoves.Select(move => new BoardAndWidth
                {
                    Board = move,
                    ParallelBranches = top.ParallelBranches
                }));
                message.Stack = stack.ToArray();
                // Republish the message with the new stack.
                string newText = JsonConvert.SerializeObject(message);
                tasks.Add(_publisherClient.PublishAsync(new PubsubMessage()
                {
                    Data = ByteString.CopyFromUtf8(newText)
                }));
            }
            else
            {
                // Branch out.
                top.ParallelBranches = nextLevelWidth;
                foreach (GameBoard move in nextMoves)
                {
                    top.Board = move;
                    // Republish the message with the new stack.
                    string newText = JsonConvert.SerializeObject(message);
                    tasks.Add(_publisherClient.PublishAsync(new PubsubMessage()
                    {
                        Data = ByteString.CopyFromUtf8(newText)
                    }));
                }
                if (message.Stack.Length > 1)
                {
                    // Pop the top.
                    message.Stack = message.Stack.SkipLast(1).ToArray();
                    // Republish the message with the new stack.
                    string newText = JsonConvert.SerializeObject(message);
                    tasks.Add(_publisherClient.PublishAsync(new PubsubMessage()
                    {
                        Data = ByteString.CopyFromUtf8(newText)
                    }));
                }
            }
            foreach (Task task in tasks) await task;
            return SubscriberClient.Reply.Ack;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriberClient.StartAsync(
                (message, token) => ProcessOneMessage(message, token));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            _subscriberClient.StopAsync(cancellationToken);
    }

    internal class BoardAndWidth
    {
        public GameBoard Board { get; set; }
        public int? ParallelBranches { get; set; }
    }

    internal class GameBoardMessage
    {
        public string SolveRequestId { get; set; }
        public BoardAndWidth[] Stack { get; set; }
    }
}