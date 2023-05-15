using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ex.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace App.Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            _logger.LogInformation("context.Resource ~ " + context.Resource?.GetType().Name);
            foreach (var requirement in requirements)
            {
                if (requirement is GenZRequirement)
                {
                    if (IsGenZ(context.User, (GenZRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                    // code xu ly de kiem tra User dam bao requirement/GenZRequirement

                }
                if (requirement is ArticleUpdateRequirement)
                {
                    if (CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object resource, ArticleUpdateRequirement requirement)
        {
            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin cap nhat");
                return true;
            }
            var article = resource as Article;
            var dateCreated = article.Created;
            var dateCanUpdate = new DateTime(requirement.Year, requirement.Month, requirement.Date);
            if (dateCreated > dateCanUpdate)
            {
                _logger.LogInformation("Qua ngay cap nhat");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;
            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} khong co ngay sinh, khong thoa man GenZRequirement");
                return false;
            }
            int year = appUser.BirthDate.Value.Year;
            var success = (year >= requirement.FromYear && year <= requirement.ToYear);
            if (success)
            {
                _logger.LogInformation($"{appUser.UserName} thoa man GenZRequirement");
            }
            else
            {
                _logger.LogInformation($"{appUser.UserName} khong thoa man GenZRequirement");
            }
            return success;
        }
    }
}