﻿using OutOfSchool.Services.Repository.Api;

namespace OutOfSchool.BusinessLogic.Services.NotificationsClearing;

public class NotificationsClearingService : INotificationsClearingService
{
    private readonly INotificationRepository notificationRepository;
    private readonly ILogger<NotificationsClearingService> logger;

    public NotificationsClearingService(
        INotificationRepository notificationRepository,
        ILogger<NotificationsClearingService> logger)
    {
        this.notificationRepository = notificationRepository;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task ClearNotifications(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Clearing old notifications was started");

        await notificationRepository.ClearNotifications();

        logger.LogDebug("Creating statistic reports was finished");
    }
}
