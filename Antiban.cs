using System.Collections.Generic;
using System.Linq;

namespace Antiban
{
    public class Antiban
    {
        private readonly List<AntibanResult> _results = new();
        private readonly RestrictionTimeLine _restrictionAllTimeLine = new();
        private readonly Dictionary<string, RestrictionTimeLineManager> _restrictionPhoneTimeLines = new();

        /// <summary>
        /// Добавление сообщений в систему, для обработки порядка сообщений
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PushEventMessage(EventMessage eventMessage)
        {
            var restrictionPhoneTimeLineManager = _restrictionPhoneTimeLines
                .GetValueOrDefault(eventMessage.Phone, new RestrictionTimeLineManager(_restrictionAllTimeLine));

            if (!_restrictionPhoneTimeLines.ContainsKey(eventMessage.Phone))
                _restrictionPhoneTimeLines.Add(eventMessage.Phone, restrictionPhoneTimeLineManager);

            var sendTime = restrictionPhoneTimeLineManager.FindFreeSpaceAndReserve(eventMessage.Priority, eventMessage.DateTime);

            _results.Add(new AntibanResult()
            {
                EventMessageId = eventMessage.Id,
                SentDateTime = sendTime
            });
        }

        /// <summary>
        /// Возвращает порядок отправок сообщений
        /// </summary>
        /// <returns></returns>
        public List<AntibanResult> GetResult()
        {
            return _results.OrderBy(r => r.SentDateTime).ToList();
        }
    }
}
