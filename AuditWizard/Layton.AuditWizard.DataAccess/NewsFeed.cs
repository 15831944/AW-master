using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
    public static class NewsFeed
    {
        public enum Priority
        {
            Fatal,
            Error,
            Warning,
            Information
        }
        public static void AddNewsItem(Priority priority, string newsText)
        {
            new NewsFeedDAO().AddNewsFeed(Convert.ToInt32(priority), newsText);
        }
    }
}
