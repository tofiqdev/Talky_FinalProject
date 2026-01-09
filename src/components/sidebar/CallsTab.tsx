export default function CallsTab() {
  const calls = [
    { id: 1, name: 'Jordan Smith', type: 'incoming', time: '10:42 AM', duration: '12:34', missed: false },
    { id: 2, name: 'Alex Rivier', type: 'outgoing', time: '9:15 AM', duration: '5:23', missed: false },
    { id: 3, name: 'Sarah Johnson', type: 'incoming', time: 'Yesterday', duration: '0:00', missed: true },
    { id: 4, name: 'Chris Evans', type: 'outgoing', time: 'Yesterday', duration: '18:45', missed: false },
    { id: 5, name: 'Taylor Reed', type: 'incoming', time: '2 days ago', duration: '3:12', missed: false },
  ];

  return (
    <div className="flex-1 overflow-y-auto">
      {calls.map((call) => (
        <div
          key={call.id}
          className="px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3"
        >
          <div className="relative flex-shrink-0">
            <div className="w-12 h-12 rounded-full bg-gray-300"></div>
          </div>
          <div className="flex-1 min-w-0">
            <div className="flex items-center justify-between mb-1">
              <h3 className={`font-semibold text-sm ${call.missed ? 'text-red-500' : 'text-gray-900'}`}>
                {call.name}
              </h3>
              <span className="text-xs text-gray-500">{call.time}</span>
            </div>
            <div className="flex items-center gap-2">
              <svg
                className={`w-4 h-4 ${call.type === 'incoming' ? 'text-green-500' : 'text-gray-500'} ${call.missed ? 'text-red-500' : ''}`}
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d={call.type === 'incoming' ? 'M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z' : 'M16 3h5m0 0v5m0-5l-6 6M5 3a2 2 0 00-2 2v1c0 8.284 6.716 15 15 15h1a2 2 0 002-2v-3.28a1 1 0 00-.684-.948l-4.493-1.498a1 1 0 00-1.21.502l-1.13 2.257a11.042 11.042 0 01-5.516-5.517l2.257-1.128a1 1 0 00.502-1.21L9.228 3.683A1 1 0 008.279 3H5z'}
                />
              </svg>
              <span className="text-sm text-gray-600">
                {call.missed ? 'Missed call' : call.duration}
              </span>
            </div>
          </div>
          <button className="text-cyan-500 hover:text-cyan-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
            </svg>
          </button>
        </div>
      ))}
    </div>
  );
}
