using System;
using Xunit;
using Snippets;

namespace Tests
{
    
    public class SecurityCenterTests
    {
        [Fact]
        public void CreateNotificationConfig_ShouldCreateConfig()
        {
            String configId = "csharp-create-config-test111";
            Assert.NotNull(CreateNotificationConfigSnippet.createNotificationConfig(GetOrganizationId(), configId, GetProjectId(), GetTopic()));
            DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
            Assert.True(true);
        }

        [Fact]
        public void DeleteNotificationConfig_ShouldDeleteConfig()
        {
            String configId = "csharp-delete-config-test111";
            CreateNotificationConfigSnippet.createNotificationConfig(GetOrganizationId(), configId, GetProjectId(), GetTopic());
            Assert.True(DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId));
        }

        [Fact]
        public void ListNotificationConfig_ShouldReturnNonEmpty()
        {
            String configId = "csharp-list-config-test111";
            CreateNotificationConfigSnippet.createNotificationConfig(GetOrganizationId(), configId, GetProjectId(), GetTopic());
            Assert.NotNull(ListNotificationConfigSnippets.listNotificationConfigs(GetOrganizationId()));
            DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
        }

        [Fact]
        public void GetNotificationConfig_ShouldReturnConfig()
        {
            String configId = "csharp-get-config-test111";
            CreateNotificationConfigSnippet.createNotificationConfig(GetOrganizationId(), configId, GetProjectId(), GetTopic());
            Assert.NotNull(GetNotificationConfigSnippets.getNotificationConfig(GetOrganizationId(), configId));
            DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
        }

        [Fact]
        public void UpdateNotificationConfig_ShouldUpdateConfig()
        {
            String configId = "csharp-update-config-test111";
            CreateNotificationConfigSnippet.createNotificationConfig(GetOrganizationId(), configId, GetProjectId(), GetTopic());
            Assert.NotNull(UpdateNotificationConfigSnippets.updateNotificationConfig(GetOrganizationId(), configId, GetProjectId(), GetTopic()));
            DeleteNotificationConfigSnippets.deleteNotificationConfig(GetOrganizationId(), configId);
        }

        private String GetOrganizationId() {
            return "1081635000895";
        }

        private String GetProjectId() {
            return "project-a-id";
        }

        private String GetTopic() {
            return "notifications-sample-topic";
        }
    }
}
