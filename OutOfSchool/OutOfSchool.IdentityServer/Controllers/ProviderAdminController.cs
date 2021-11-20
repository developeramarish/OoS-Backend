﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using OutOfSchool.Common;
using OutOfSchool.Common.Extensions;
using OutOfSchool.Common.Models;
using OutOfSchool.EmailSender;
using OutOfSchool.IdentityServer.Services.Password;
using OutOfSchool.Services;
using OutOfSchool.Services.Models;
using OutOfSchool.Services.Repository;

namespace OutOfSchool.IdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProviderAdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IProviderAdminRepository providerAdminRepository;
        private readonly IEmailSender emailSender;
        private readonly ILogger<ProviderAdminController> logger;
        private readonly OutOfSchoolDbContext db;
        private readonly IMapper mapper;
        private readonly ResponseDto response;
        private string path;
        private string userId;

        public ProviderAdminController(
            UserManager<User> userManager,
            IProviderAdminRepository providerAdminRepository,
            IEmailSender emailSender,
            ILogger<ProviderAdminController> logger,
            OutOfSchoolDbContext db,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.response = new ResponseDto();
            this.providerAdminRepository = providerAdminRepository;
            this.db = db;
            this.emailSender = emailSender;
            this.logger = logger;
            this.mapper = mapper;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            path = $"{context.HttpContext.Request.Path.Value}[{context.HttpContext.Request.Method}]";
            userId = User.GetUserPropertyByClaimType(IdentityResourceClaimsTypes.Sub);
        }

        [HttpPost]
        [Authorize(Roles = "provider, provideradmin")]
        public async Task<ResponseDto> Create(ProviderAdminDto providerAdminDto)
        {
            logger.LogDebug($"Received request " +
                $"{Request.Headers["X-Request-ID"]}. {path} started. User(id): {userId}");

            var user = mapper.Map<User>(providerAdminDto);

            var password = PasswordGenerator
                .GenerateRandomPassword(userManager.Options.Password);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    IdentityResult result = await userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        transaction.Rollback();

                        logger.LogError($"{path} Error happened while creation ProviderAdmin. Request(id): {Request.Headers["X-Request-ID"]}" +
                            $"User(id): {userId}" +
                            $"{string.Join(System.Environment.NewLine, result.Errors.Select(e => e.Description))}");

                        response.IsSuccess = false;
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;

                        return response;
                    }

                    var roleAssignResult = await userManager.AddToRoleAsync(user, user.Role);

                    if (!roleAssignResult.Succeeded)
                    {
                        transaction.Rollback();

                        logger.LogError($"{path} Error happened while adding role to user. Request(id): {Request.Headers["X-Request-ID"]}" +
                            $"User(id): {userId}" +
                            $"{string.Join(System.Environment.NewLine, result.Errors.Select(e => e.Description))}");

                        response.IsSuccess = false;
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;

                        return response;
                    }

                    providerAdminDto.UserId = user.Id;

                    var providerAdmin = mapper.Map<ProviderAdmin>(providerAdminDto);

                    await providerAdminRepository.Create(providerAdmin)
                            .ConfigureAwait(false);

                    response.IsSuccess = true;
                    response.Result = providerAdminDto;

                    transaction.Commit();

                    // TODO:
                    // Endpoint with sending new password

                    // TODO:
                    // Use template instead
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("EmailConfirmation", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    var subject = "Запрошення!";
                    var htmlMessage = $"Для реєстрації на платформі перейдіть " +
                        $"за посиланням та заповність ваші данні в особистому кабінеті.<br>" +
                        $"{confirmationLink}<br><br>" +
                        $"Логін: {user.Email}<br>" +
                        $"Пароль: {password}";

                    await emailSender.SendAsync(user.Email, subject, htmlMessage);

                    return response;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    logger.LogError($"{path} {ex.Message} User(id): {userId}.");

                    response.IsSuccess = false;
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;

                    return response;
                }
            }
        }

        [HttpDelete]
        [Authorize(Roles = "provider")]
        public async Task<ResponseDto> Delete(DeleteProviderAdminDto deleteProviderAdminDto)
        {
            logger.LogDebug($"Received request " +
                $"{Request.Headers["X-Request-ID"]}. {path} started. User(id): {userId}");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var providerAdmin = db.ProviderAdmins
                        .SingleOrDefault(pa => pa.UserId == deleteProviderAdminDto.ProviderAdminId);

                    if (providerAdmin is null)
                    {
                        response.IsSuccess = false;
                        response.HttpStatusCode = HttpStatusCode.NotFound;
                    }

                    db.ProviderAdmins.Remove(providerAdmin);

                    var user = await userManager.FindByIdAsync(deleteProviderAdminDto.ProviderAdminId);
                    var result = await userManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        transaction.Rollback();

                        logger.LogError($"{path} Error happened while deleting ProviderAdmin. Request(id): {Request.Headers["X-Request-ID"]}" +
                            $"User(id): {userId}" +
                            $"{string.Join(System.Environment.NewLine, result.Errors.Select(e => e.Description))}");

                        response.IsSuccess = false;
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;

                        return response;
                    }

                    response.IsSuccess = true;
                    response.HttpStatusCode = HttpStatusCode.OK;

                    transaction.Commit();

                    return response;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    logger.LogError($"{path} Error happened while deleting ProviderAdmin. Request(id): {Request.Headers["X-Request-ID"]}" +
                            $"User(id): {userId} {ex.Message}");

                    response.IsSuccess = false;
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;

                    return response;
                }
            }
        }
    }
}