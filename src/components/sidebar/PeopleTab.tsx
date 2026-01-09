export default function PeopleTab() {
  const people = [
    { id: 1, name: 'Jordan Smith', status: 'Online', isOnline: true },
    { id: 2, name: 'Alex Rivier', status: 'Online', isOnline: true },
    { id: 3, name: 'Sarah Johnson', status: 'Last seen 2h ago', isOnline: false },
    { id: 4, name: 'Chris Evans', status: 'Last seen yesterday', isOnline: false },
    { id: 5, name: 'Taylor Reed', status: 'Last seen 3 days ago', isOnline: false },
    { id: 6, name: 'Maria Garcia', status: 'Last seen 1 week ago', isOnline: false },
  ];

  return (
    <div className="flex-1 overflow-y-auto">
      <div className="px-5 py-3 border-b border-gray-100">
        <input
          type="text"
          placeholder="Search contacts..."
          className="w-full px-4 py-2 bg-gray-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500"
        />
      </div>
      {people.map((person) => (
        <div
          key={person.id}
          className="px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3"
        >
          <div className="relative flex-shrink-0">
            <div className="w-12 h-12 rounded-full bg-gray-300"></div>
            {person.isOnline && (
              <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
            )}
          </div>
          <div className="flex-1 min-w-0">
            <h3 className="font-semibold text-gray-900 text-sm">{person.name}</h3>
            <p className={`text-sm ${person.isOnline ? 'text-green-500' : 'text-gray-500'}`}>
              {person.status}
            </p>
          </div>
          <button className="text-cyan-500 hover:text-cyan-600">
            <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
              <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
            </svg>
          </button>
        </div>
      ))}
    </div>
  );
}
