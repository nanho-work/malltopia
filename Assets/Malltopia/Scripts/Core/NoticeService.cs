using System.Collections.Generic;

namespace Malltopia
{
    public class NoticeService
    {
        private readonly HashSet<string> seenNoticeIds = new HashSet<string>();

        public NoticeService()
        {
        }

        public NoticeService(IEnumerable<string> initialSeenNoticeIds)
        {
            if (initialSeenNoticeIds == null)
            {
                return;
            }

            foreach (var noticeId in initialSeenNoticeIds)
            {
                MarkSeen(noticeId);
            }
        }

        public bool IsNew(string noticeId)
        {
            return !string.IsNullOrEmpty(noticeId) && !seenNoticeIds.Contains(noticeId);
        }

        public void MarkSeen(string noticeId)
        {
            if (!string.IsNullOrEmpty(noticeId))
            {
                seenNoticeIds.Add(noticeId);
            }
        }

        public string[] ExportSeenNoticeIds()
        {
            var values = new string[seenNoticeIds.Count];
            seenNoticeIds.CopyTo(values);
            return values;
        }
    }
}
