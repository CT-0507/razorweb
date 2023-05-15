using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public ArticleUpdateRequirement(int year = 2021, int month = 6, int date = 30)
        {
            Year = year;
            Month = month;
            Date = date;
        }

        public int Year { set; get; }
        public int Month { set; get; }
        public int Date { set; get; }
    }
}