using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DDay.iCal;
using DDay.iCal.Serialization;
using DDay.iCal.Serialization.iCalendar;
using Event = CalendarFeedSpike.Models.Event;

namespace CalendarFeedSpike.Controllers
{
    [RoutePrefix("api/user")]
    public class CalendarFeedController : ApiController
    {
        private readonly IMapper _Mapper;

        public CalendarFeedController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Event, DDay.iCal.Event>()
                    .ForMember(dest => dest.DTStart, opt =>
                    {
                        //opt.MapFrom(src => src.DTStart);
                        opt.ResolveUsing(src => new iCalDateTime(src.DTStart));

                    })
                    .ForMember(dest => dest.DTEnd, opt =>
                    {
                        //opt.MapFrom(src => src.DTEnd);
                        opt.ResolveUsing(src => new iCalDateTime(src.DTEnd));
                    })
                    .ReverseMap();
            });

            _Mapper = config.CreateMapper();
        }

        [Route("{userId:int}/calendar")]
        public HttpResponseMessage Get(int userId)
        {
            if (userId != 12345)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "The user does not exist"
                };
            }

            var events = new List<Event>
            {
                new Event
                {
                    Description = "A test event for test purposes",
                    Location = "1450 Suwannee Lane, Benton, LA, 71006",
                    Summary = "Test Appointment #1",
                    DTStart = DateTime.UtcNow,
                    DTEnd = DateTime.UtcNow.AddHours(3)
                },
                new Event
                {
                    Description = "A test event for test purposes",
                    Location = "1450 Suwannee Lane, Benton, LA, 71006",
                    Summary = "Test Appointment #2",
                    DTStart = DateTime.UtcNow.AddDays(1),
                    DTEnd = DateTime.UtcNow.AddDays(1).AddHours(3)
                }
            };

            var calendar = new iCalendar();

            foreach (var entry in events)
            {
                var evt = calendar.Create<DDay.iCal.Event>();
                _Mapper.Map<CalendarFeedSpike.Models.Event, DDay.iCal.Event>(entry, evt);
            }

            var context = new SerializationContext();
            var serializationFactory = new SerializerFactory();

            var serializer = serializationFactory.Build(calendar.GetType(), context) as IStringSerializer;
            if (serializer == null)
            {
                var message = $"Couldn't cast object to IStringSerializer";
                throw new InvalidOperationException(message);
            }

            return new HttpResponseMessage
            {
                Content = new StringContent(serializer.SerializeToString(calendar), Encoding.UTF8, "text/calendar"),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
