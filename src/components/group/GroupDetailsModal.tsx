import { useState } from 'react';
import type { Group } from '../../types/group';
import { useAuthStore } from '../../store/authStore';
import { useChatStore } from '../../store/chatStore';

interface GroupDetailsModalProps {
  group: Group;
  isOpen: boolean;
  onClose: () => void;
  onUpdate: () => void;
  onDelete?: () => void;
}

export default function GroupDetailsModal({ group, isOpen, onClose, onUpdate, onDelete }: GroupDetailsModalProps) {
  const { user } = useAuthStore();
  const { users } = useChatStore();
  const [showAddMember, setShowAddMember] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [showSettings, setShowSettings] = useState(false);

  const isOwner = user?.id === group.createdById;
  const currentMember = group.members.find(m => m.userId === user?.id);
  const isAdmin = currentMember?.isAdmin || false;
  const canManage = isOwner || isAdmin;

  // Get users not in group
  const availableUsers = users.filter(u => !group.members.some(m => m.userId === u.id));

  const handlePromote = async (memberId: number) => {
    if (!canManage) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/members/${memberId}/promote`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error('Failed to promote member');
      
      onUpdate();
    } catch (error) {
      console.error('Failed to promote member:', error);
      alert('Failed to promote member');
    } finally {
      setIsLoading(false);
    }
  };

  const handleDemote = async (memberId: number) => {
    if (!isOwner) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/members/${memberId}/demote`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error('Failed to demote member');
      
      onUpdate();
    } catch (error) {
      console.error('Failed to demote member:', error);
      alert('Failed to demote member');
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemove = async (memberId: number) => {
    if (!canManage) return;
    
    if (!confirm('Are you sure you want to remove this member?')) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/members/${memberId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error('Failed to remove member');
      
      onUpdate();
    } catch (error) {
      console.error('Failed to remove member:', error);
      alert('Failed to remove member');
    } finally {
      setIsLoading(false);
    }
  };

  const handleMute = async (memberId: number) => {
    if (!canManage) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/members/${memberId}/mute`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error('Failed to mute member');
      
      onUpdate();
    } catch (error) {
      console.error('Failed to mute member:', error);
      alert('Failed to mute member');
    } finally {
      setIsLoading(false);
    }
  };

  const handleUnmute = async (memberId: number) => {
    if (!canManage) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/members/${memberId}/unmute`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error('Failed to unmute member');
      
      onUpdate();
    } catch (error) {
      console.error('Failed to unmute member:', error);
      alert('Failed to unmute member');
    } finally {
      setIsLoading(false);
    }
  };

  const handleAddMember = async () => {
    if (!canManage || !selectedUserId) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/members`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(selectedUserId)
      });

      if (!response.ok) throw new Error('Failed to add member');
      
      setShowAddMember(false);
      setSelectedUserId(null);
      onUpdate();
    } catch (error) {
      console.error('Failed to add member:', error);
      alert('Failed to add member');
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteGroup = async () => {
    if (!isOwner) return;
    
    if (!confirm('Are you sure you want to delete this group? This action cannot be undone.')) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const errorData = await response.text();
        console.error('Delete error:', errorData);
        throw new Error(`Failed to delete group: ${response.status} ${errorData}`);
      }
      
      alert('Group deleted successfully');
      onDelete?.();
      onClose();
    } catch (error) {
      console.error('Failed to delete group:', error);
      alert(`Failed to delete group: ${error instanceof Error ? error.message : 'Unknown error'}`);
    } finally {
      setIsLoading(false);
    }
  };

  const handleLeaveGroup = async () => {
    if (isOwner) {
      alert('As the owner, you cannot leave the group. You must delete it instead.');
      return;
    }
    
    if (!confirm('Are you sure you want to leave this group?')) return;
    
    setIsLoading(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${group.id}/leave`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error('Failed to leave group');
      
      alert('Left group successfully');
      onDelete?.();
      onClose();
    } catch (error) {
      console.error('Failed to leave group:', error);
      alert('Failed to leave group');
    } finally {
      setIsLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">
            {showSettings ? 'Group Settings' : 'Group Details'}
          </h3>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {!showSettings ? (
          <>
            {/* Group Info */}
            <div className="mb-6">
              <div className="flex items-center gap-3 mb-3">
                <div className="w-16 h-16 rounded-full bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center text-white text-2xl font-bold">
                  {group.name.charAt(0).toUpperCase()}
                </div>
                <div>
                  <h4 className="font-bold text-lg">{group.name}</h4>
                  <p className="text-sm text-gray-500">{group.memberCount} members</p>
                </div>
              </div>
              {group.description && (
                <p className="text-sm text-gray-600">{group.description}</p>
              )}
            </div>

            {/* Members List */}
            <div className="mb-4">
          <div className="flex items-center justify-between mb-2">
            <h4 className="font-semibold text-sm text-gray-700">Members</h4>
            {canManage && (
              <button
                onClick={() => setShowAddMember(!showAddMember)}
                className="text-cyan-500 hover:text-cyan-600 text-sm font-medium"
              >
                + Add Member
              </button>
            )}
          </div>

          {/* Add Member Section */}
          {showAddMember && canManage && (
            <div className="mb-3 p-3 bg-gray-50 rounded-lg">
              <select
                value={selectedUserId || ''}
                onChange={(e) => setSelectedUserId(Number(e.target.value))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg mb-2"
              >
                <option value="">Select a user...</option>
                {availableUsers.map(u => (
                  <option key={u.id} value={u.id}>{u.username}</option>
                ))}
              </select>
              <div className="flex gap-2">
                <button
                  onClick={handleAddMember}
                  disabled={!selectedUserId || isLoading}
                  className="flex-1 py-1.5 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 disabled:opacity-50 text-sm"
                >
                  Add
                </button>
                <button
                  onClick={() => {
                    setShowAddMember(false);
                    setSelectedUserId(null);
                  }}
                  className="flex-1 py-1.5 border border-gray-300 rounded-lg hover:bg-gray-50 text-sm"
                >
                  Cancel
                </button>
              </div>
            </div>
          )}

          <div className="space-y-2 max-h-60 overflow-y-auto">
            {group.members.map((member) => {
              const isMemberOwner = member.userId === group.createdById;
              const canManageMember = canManage && !isMemberOwner && member.userId !== user?.id;

              return (
                <div
                  key={member.id}
                  className="flex items-center justify-between p-2 hover:bg-gray-50 rounded-lg"
                >
                  <div className="flex items-center gap-2">
                    <div className="relative">
                      <div className="w-10 h-10 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold">
                        {member.username.charAt(0).toUpperCase()}
                      </div>
                      {member.isOnline && (
                        <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
                      )}
                    </div>
                    <div>
                      <p className="font-medium text-sm">{member.username}</p>
                      <p className="text-xs text-gray-500">
                        {isMemberOwner ? (
                          <span className="text-purple-600 font-semibold">Owner</span>
                        ) : member.isAdmin ? (
                          <span className="text-cyan-600 font-semibold">Admin</span>
                        ) : member.isMuted ? (
                          <span className="text-red-600 font-semibold">Muted</span>
                        ) : (
                          'Member'
                        )}
                      </p>
                    </div>
                  </div>

                  {canManageMember && (
                    <div className="flex gap-1">
                      {!member.isMuted ? (
                        <button
                          onClick={() => handleMute(member.userId)}
                          disabled={isLoading}
                          className="px-2 py-1 text-xs bg-orange-100 text-orange-700 rounded hover:bg-orange-200 disabled:opacity-50"
                          title="Mute Member"
                        >
                          Mute
                        </button>
                      ) : (
                        <button
                          onClick={() => handleUnmute(member.userId)}
                          disabled={isLoading}
                          className="px-2 py-1 text-xs bg-green-100 text-green-700 rounded hover:bg-green-200 disabled:opacity-50"
                          title="Unmute Member"
                        >
                          Unmute
                        </button>
                      )}
                      {!member.isAdmin && isOwner && (
                        <button
                          onClick={() => handlePromote(member.userId)}
                          disabled={isLoading}
                          className="px-2 py-1 text-xs bg-cyan-100 text-cyan-700 rounded hover:bg-cyan-200 disabled:opacity-50"
                          title="Promote to Admin"
                        >
                          Promote
                        </button>
                      )}
                      {member.isAdmin && isOwner && (
                        <button
                          onClick={() => handleDemote(member.userId)}
                          disabled={isLoading}
                          className="px-2 py-1 text-xs bg-gray-100 text-gray-700 rounded hover:bg-gray-200 disabled:opacity-50"
                          title="Remove Admin"
                        >
                          Demote
                        </button>
                      )}
                      <button
                        onClick={() => handleRemove(member.userId)}
                        disabled={isLoading}
                        className="px-2 py-1 text-xs bg-red-100 text-red-700 rounded hover:bg-red-200 disabled:opacity-50"
                        title="Remove from Group"
                      >
                        Remove
                      </button>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>

        <div className="flex gap-3">
          <button
            onClick={() => setShowSettings(true)}
            className="flex-1 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition"
          >
            Settings
          </button>
          <button
            onClick={onClose}
            className="flex-1 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition"
          >
            Close
          </button>
        </div>
      </>
    ) : (
      <>
        {/* Settings View */}
        <div className="space-y-3 mb-6">
          {/* Manage Members */}
          {canManage && (
            <button
              onClick={() => setShowSettings(false)}
              className="w-full px-4 py-3 text-left hover:bg-gray-50 rounded-lg transition flex items-center gap-3"
            >
              <svg className="w-5 h-5 text-cyan-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              <div>
                <p className="font-medium text-gray-900">Manage Members</p>
                <p className="text-xs text-gray-500">Add, remove, or promote members</p>
              </div>
            </button>
          )}

          {/* Leave Group */}
          {!isOwner && (
            <button
              onClick={handleLeaveGroup}
              disabled={isLoading}
              className="w-full px-4 py-3 text-left hover:bg-red-50 rounded-lg transition flex items-center gap-3 disabled:opacity-50"
            >
              <svg className="w-5 h-5 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
              <div>
                <p className="font-medium text-red-600">Leave Group</p>
                <p className="text-xs text-red-400">You will no longer be a member</p>
              </div>
            </button>
          )}

          {/* Delete Group (Owner Only) */}
          {isOwner && (
            <button
              onClick={handleDeleteGroup}
              disabled={isLoading}
              className="w-full px-4 py-3 text-left hover:bg-red-50 rounded-lg transition flex items-center gap-3 disabled:opacity-50"
            >
              <svg className="w-5 h-5 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              <div>
                <p className="font-medium text-red-600">Delete Group</p>
                <p className="text-xs text-red-400">Permanently delete this group</p>
              </div>
            </button>
          )}
        </div>

        <button
          onClick={() => setShowSettings(false)}
          className="w-full py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition"
        >
          Back to Details
        </button>
      </>
    )}
      </div>
    </div>
  );
}