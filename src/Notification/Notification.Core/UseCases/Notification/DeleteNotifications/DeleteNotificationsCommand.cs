using MediatR;
using System;

namespace Planorama.Notification.Core.UseCases.Notification.DeleteNotifications
{
    public class DeleteNotificationsCommand : IRequest
    {
        public Guid? UserId { get; set; }
        public string DeleteId { get; set; }

        public DeleteNotificationsCommand(Guid? userId, string deleteId)
        {
            UserId = userId;
            DeleteId = deleteId;
        }

        public DeleteNotificationsCommand(string deleteId)
        {
            DeleteId = deleteId;
        }
    }
}
