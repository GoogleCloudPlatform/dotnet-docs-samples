using System;
using System.Collections;
using System.Configuration;
using System.Threading;
using System.Transactions;
using PetShop.BLL;
using PetShop.Model;

namespace PetShop.OrderProcessor {
    /// <summary>
    /// We created this processor as a console application just to illustrate how to launch background threats
    /// to process Orders from MSMQ.  In a real application, we would implement this functionality as a Windows Services.
    /// </summary>
    class Program {
        // Variables from App Settings
        private static int transactionTimeout = int.Parse(ConfigurationManager.AppSettings["TransactionTimeout"]);
        private static int queueTimeout = int.Parse(ConfigurationManager.AppSettings["QueueTimeout"]);
        private static int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
        private static int threadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCount"]);

        private static int totalOrdersProcessed = 0;

        static void Main() {

            Thread workTicketThread;
            Thread[] workerThreads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++) {

                workTicketThread = new Thread(new ThreadStart(ProcessOrders));

                // Make this a background thread, so it will terminate when the main thread/process is de-activated
                workTicketThread.IsBackground = true;
                workTicketThread.SetApartmentState(ApartmentState.STA);

                // Start the Work
                workTicketThread.Start();
                workerThreads[i] = workTicketThread;
            }

            Console.WriteLine("Processing started. Press Enter to stop.");
            Console.ReadLine();
            Console.WriteLine("Aborting Threads. Press wait...");

            //abort all threads
            for (int i = 0; i < workerThreads.Length; i++) {

                workerThreads[i].Abort();
            }

            Console.WriteLine();
            Console.WriteLine(totalOrdersProcessed + " Orders processed.");
            Console.WriteLine("Processing stopped. Press Enter to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Process a batch of asynchronous orders from the queue and submit them to the database within a transaction
        /// </summary>
        private static void ProcessOrders() {

            // the transaction timeout should be long enough to handle all of orders in the batch
            TimeSpan tsTimeout = TimeSpan.FromSeconds(Convert.ToDouble(transactionTimeout * batchSize));

            Order order = new Order();
            while (true) {

                // queue timeout variables
                TimeSpan datetimeStarting = new TimeSpan(DateTime.Now.Ticks);
                double elapsedTime = 0;

                int processedItems = 0;

                ArrayList queueOrders = new ArrayList();

                //OrderInfo orderData = orderQueue.Receive(timeout);
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tsTimeout)) {
                    // Receive the orders from the queue
                    for (int j = 0; j < batchSize; j++) {

                        try {
                            //only receive more queued orders if there is enough time
                            if ((elapsedTime + queueTimeout + transactionTimeout) < tsTimeout.TotalSeconds) {
                                queueOrders.Add(order.ReceiveFromQueue(queueTimeout));
                            }
                            else {
                                j = batchSize;   // exit loop
                            }

                            //update elapsed time
                            elapsedTime = new TimeSpan(DateTime.Now.Ticks).TotalSeconds - datetimeStarting.TotalSeconds;
                        }
                        catch (TimeoutException) {

                            //exit loop because no more messages are waiting
                            j = batchSize;
                        }
                    }

                    //process the queued orders
                    for (int k = 0; k < queueOrders.Count; k++) {
                        order.Insert((OrderInfo)queueOrders[k]);
                        processedItems++;
                        totalOrdersProcessed++;
                    }

                    //batch complete or MSMQ receive timed out
                    ts.Complete();
                }

                Console.WriteLine("(Thread Id " + Thread.CurrentThread.ManagedThreadId + ") batch finished, " + processedItems + " items, in " + elapsedTime.ToString() + " seconds.");
            }
        }
    }
}
