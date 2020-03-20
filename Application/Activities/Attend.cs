using System;
using System.Linq;
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
    public class Attend
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
                if (userActivity != null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Attendence = "Already attending this activity" });
                }

                var attendee = new UserActivity
                {
                    Activity = acitivity,
                    AppUser = user,
                    IsHost = false,
                    DateJoined = DateTime.Now,
                };

                await _context.UserActivities.AddAsync(attendee);

                var success = await _context.SaveChangesAsync() > 0;
                if (success) return Unit.Value;
                throw new Exception("Problem saving changes");
            }
        }
    }
}