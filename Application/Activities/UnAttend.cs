using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UnAttend
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());

                var acitivity = await _context.Activities.FindAsync(request.Id);
                if (acitivity == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "activity is not exist" });
                }

                var userActivity = await _context.UserActivities.SingleOrDefaultAsync(x => x.AppUserId == user.Id && x.ActivityId == acitivity.Id);
                if (userActivity == null)
                {
                    return Unit.Value;
                }

                if (userActivity.IsHost)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Attendance = "You can't remove youself as Host" });
                }
                _context.UserActivities.Remove(userActivity);

                var success = await _context.SaveChangesAsync() > 0;
                if (success) return Unit.Value;
                throw new Exception("Problem saving changes");
            }
        }

    }
}