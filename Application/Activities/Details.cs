using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        public class Qurey : IRequest<ActivityDto>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Qurey, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ActivityDto> Handle(Qurey request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    // .Include(act => act.UserActivities)
                    // .ThenInclude(ua => ua.AppUser)
                    // .SingleOrDefaultAsync(act => act.Id == request.Id);
                    .FindAsync(request.Id);
                if (activity == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not found" });
                }

                return _mapper.Map<Activity, ActivityDto>(activity);
            }
        }
    }
}