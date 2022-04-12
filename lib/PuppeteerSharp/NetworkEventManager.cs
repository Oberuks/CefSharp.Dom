using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CefSharp.Puppeteer.Messaging;

namespace CefSharp.Puppeteer
{
    internal class NetworkEventManager
    {
        private readonly ConcurrentDictionary<string, RequestWillBeSentPayload> _requestWillBeSentMap = new ConcurrentDictionary<string, RequestWillBeSentPayload>();
        private readonly ConcurrentDictionary<string, FetchRequestPausedResponse> _requestPausedMap = new ConcurrentDictionary<string, FetchRequestPausedResponse>();
        private readonly ConcurrentDictionary<string, Request> _httpRequestsMap = new ConcurrentDictionary<string, Request>();
        private readonly ConcurrentDictionary<string, QueuedEventGroup> _queuedEventGroupMap = new ConcurrentDictionary<string, QueuedEventGroup>();
        private readonly ConcurrentDictionary<string, List<RedirectInfo>> _queuedRedirectInfoMap = new ConcurrentDictionary<string, List<RedirectInfo>>();
        private readonly ConcurrentDictionary<string, List<ResponseReceivedExtraInfoResponse>> _responseReceivedExtraInfoMap = new ConcurrentDictionary<string, List<ResponseReceivedExtraInfoResponse>>();

        internal void Forget(string requestId)
        {
            _requestWillBeSentMap.TryRemove(requestId, out _);
            _requestPausedMap.TryRemove(requestId, out _);
            _queuedEventGroupMap.TryRemove(requestId, out _);
            _queuedRedirectInfoMap.TryRemove(requestId, out _);
            _responseReceivedExtraInfoMap.TryRemove(requestId, out _);
        }

        internal List<ResponseReceivedExtraInfoResponse> ResponseExtraInfo(string networkRequestId)
        {
            if (!_responseReceivedExtraInfoMap.ContainsKey(networkRequestId))
            {
                _responseReceivedExtraInfoMap.AddOrUpdate(
                    networkRequestId,
                    new List<ResponseReceivedExtraInfoResponse>(),
                    (_, __) => new List<ResponseReceivedExtraInfoResponse>());
            }
            _responseReceivedExtraInfoMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        private List<RedirectInfo> QueuedRedirectInfo(string fetchRequestId)
        {
            if (!_queuedRedirectInfoMap.ContainsKey(fetchRequestId))
            {
                _queuedRedirectInfoMap.TryAdd(fetchRequestId, new List<RedirectInfo>());
            }
            _queuedRedirectInfoMap.TryGetValue(fetchRequestId, out var result);
            return result;
        }

        internal void QueueRedirectInfo(string fetchRequestId, RedirectInfo redirectInfo)
            => QueuedRedirectInfo(fetchRequestId).Add(redirectInfo);

        internal RedirectInfo TakeQueuedRedirectInfo(string fetchRequestId)
        {
            var list = QueuedRedirectInfo(fetchRequestId);
            var result = list.FirstOrDefault();

            if (result != null)
            {
                list.Remove(result);
            }

            return result;
        }

        public int NumRequestsInProgress
            => _httpRequestsMap.Values.Count(r => r.Response == null);

        internal ResponseReceivedExtraInfoResponse ShiftResponseExtraInfo(string networkRequestId)
        {
            if (!_responseReceivedExtraInfoMap.ContainsKey(networkRequestId))
            {
                _responseReceivedExtraInfoMap.TryAdd(networkRequestId, new List<ResponseReceivedExtraInfoResponse>());
            }

            _responseReceivedExtraInfoMap.TryGetValue(networkRequestId, out var list);
            var result = list.FirstOrDefault();

            if (result != null)
            {
                list.Remove(result);
            }

            return result;
        }

        internal void StoreRequestWillBeSent(string networkRequestId, RequestWillBeSentPayload e)
            => _requestWillBeSentMap.AddOrUpdate(networkRequestId, e, (_, __) => e);

        internal RequestWillBeSentPayload GetRequestWillBeSent(string networkRequestId)
        {
            _requestWillBeSentMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void ForgetRequestWillBeSent(string networkRequestId)
            => _requestWillBeSentMap.TryRemove(networkRequestId, out _);

        internal FetchRequestPausedResponse GetRequestPaused(string networkRequestId)
        {
            _requestPausedMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void ForgetRequestPaused(string networkRequestId)
            => _requestPausedMap.TryRemove(networkRequestId, out _);

        internal void StoreRequestPaused(string networkRequestId, FetchRequestPausedResponse e)
            => _requestPausedMap.AddOrUpdate(networkRequestId, e, (_, __) => e);

        internal Request GetRequest(string networkRequestId)
        {
            _httpRequestsMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void StoreRequest(string networkRequestId, Request request)
            => _httpRequestsMap.AddOrUpdate(networkRequestId, request, (_, __) => request);

        internal void ForgetRequest(string requestId)
            => _requestWillBeSentMap.TryRemove(requestId, out _);

        internal void QueuedEventGroup(string networkRequestId, QueuedEventGroup group)
            => _queuedEventGroupMap.AddOrUpdate(networkRequestId, group, (_, __) => group);

        internal QueuedEventGroup GetQueuedEventGroup(string networkRequestId)
        {
            _queuedEventGroupMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        // Puppeteer doesn't have this. but it seems that .NET needs this to avoid race conditions
        internal void ForgetQueuedEventGroup(string networkRequestId)
            => _queuedEventGroupMap.TryRemove(networkRequestId, out _);
    }
}
