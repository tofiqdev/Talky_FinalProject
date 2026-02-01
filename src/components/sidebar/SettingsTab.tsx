import { useState, useEffect } from 'react';
import { useAuthStore } from '../../store/authStore';
import { useNavigate } from 'react-router-dom';

export default function SettingsTab() {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();
  const [showEditProfile, setShowEditProfile] = useState(false);
  const [showAccount, setShowAccount] = useState(false);
  const [showChats, setShowChats] = useState(false);
  const [showNotifications, setShowNotifications] = useState(false);
  const [showStorage, setShowStorage] = useState(false);
  const [showHelp, setShowHelp] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div className="flex-1 overflow-y-auto">
      {/* Profile Section */}
      <div className="px-5 py-6 border-b border-gray-100">
        <div className="flex items-center gap-4 mb-4">
          {user?.avatar ? (
            <img 
              src={user.avatar} 
              alt={user.username}
              className="w-20 h-20 rounded-full object-cover"
            />
          ) : (
            <div className="w-20 h-20 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white text-2xl font-bold">
              {user?.username.charAt(0) || 'U'}
            </div>
          )}
          <div>
            <h3 className="font-bold text-lg text-gray-900">{user?.username || 'User'}</h3>
            <p className="text-sm text-gray-600">{user?.email || 'user@example.com'}</p>
          </div>
        </div>
        <button 
          onClick={() => setShowEditProfile(true)}
          className="w-full py-2 px-4 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition"
        >
          Edit Profile
        </button>
      </div>

      {/* Settings Options */}
      <div className="py-2">
        <SettingItem
          onClick={() => setShowAccount(true)}
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          }
          title="Account"
          subtitle="Privacy, security, change number"
        />
        <SettingItem
          onClick={() => setShowChats(true)}
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
          }
          title="Chats"
          subtitle="Theme, wallpapers, chat history"
        />
        <SettingItem
          onClick={() => setShowNotifications(true)}
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
          }
          title="Notifications"
          subtitle="Message, group & call tones"
        />
        <SettingItem
          onClick={() => setShowStorage(true)}
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
          }
          title="Storage and data"
          subtitle="Network usage, auto-download"
        />
        <SettingItem
          onClick={() => setShowHelp(true)}
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          }
          title="Help"
          subtitle="Help center, contact us, privacy policy"
        />
        
        <div className="border-t border-gray-100 mt-2 pt-2">
          <button
            onClick={handleLogout}
            className="w-full px-5 py-3 flex items-center gap-3 hover:bg-red-50 transition text-red-600"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            <span className="font-medium">Logout</span>
          </button>
        </div>
      </div>

      {/* Modals */}
      {showEditProfile && <EditProfileModal onClose={() => setShowEditProfile(false)} />}
      {showAccount && <AccountModal onClose={() => setShowAccount(false)} />}
      {showChats && <ChatsModal onClose={() => setShowChats(false)} />}
      {showNotifications && <NotificationsModal onClose={() => setShowNotifications(false)} />}
      {showStorage && <StorageModal onClose={() => setShowStorage(false)} />}
      {showHelp && <HelpModal onClose={() => setShowHelp(false)} />}
    </div>
  );
}

function SettingItem({ icon, title, subtitle, onClick }: { icon: React.ReactNode; title: string; subtitle: string; onClick: () => void }) {
  return (
    <button onClick={onClick} className="w-full px-5 py-3 flex items-center gap-3 hover:bg-gray-50 transition">
      <div className="text-gray-600">{icon}</div>
      <div className="flex-1 text-left">
        <h4 className="font-medium text-gray-900 text-sm">{title}</h4>
        <p className="text-xs text-gray-500">{subtitle}</p>
      </div>
      <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
      </svg>
    </button>
  );
}


// Edit Profile Modal
function EditProfileModal({ onClose }: { onClose: () => void }) {
  const { user, setAuth } = useAuthStore();
  const [username, setUsername] = useState(user?.username || '');
  const [email, setEmail] = useState(user?.email || '');
  const [uploading, setUploading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [previewImage, setPreviewImage] = useState<string | null>(user?.avatar || null);

  // Update preview when user changes
  useEffect(() => {
    if (user?.avatar && !previewImage) {
      setPreviewImage(user.avatar);
    }
  }, [user?.avatar, previewImage]);

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!file.type.startsWith('image/')) {
      alert('Please select an image file');
      return;
    }

    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      alert('Image size must be less than 5MB');
      return;
    }

    setUploading(true);

    try {
      // Compress and convert to base64
      const base64String = await compressImage(file);
      setPreviewImage(base64String);

      console.log('Uploading profile picture, size:', base64String.length);

      // Upload to backend
      const token = localStorage.getItem('token');
      console.log('Token:', token ? 'exists' : 'missing');
      console.log('Request URL:', '/api/User/profile-picture');
      console.log('Request body:', JSON.stringify({ avatar: base64String.substring(0, 50) + '...' }));
      
      const response = await fetch('/api/User/profile-picture', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        },
        body: JSON.stringify({ avatar: base64String })
      });

      console.log('Response status:', response.status);
      console.log('Response ok:', response.ok);

      if (!response.ok) {
        const errorText = await response.text();
        console.error('Error response:', errorText);
        throw new Error(`Failed to upload profile picture: ${response.status} - ${errorText}`);
      }

      const updatedUser = await response.json();
      console.log('Profile picture updated, user:', updatedUser);
      console.log('Has avatar:', !!updatedUser.avatar);
      
      // Update auth store
      if (token) {
        setAuth(updatedUser, token);
        console.log('Auth store updated');
      }

      alert('Profile picture updated successfully!');
    } catch (error) {
      console.error('Error uploading profile picture:', error);
      alert('Failed to upload profile picture');
    } finally {
      setUploading(false);
    }
  };

  const compressImage = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = (e) => {
        const img = new Image();
        img.onload = () => {
          const canvas = document.createElement('canvas');
          let width = img.width;
          let height = img.height;
          
          // Max dimensions for profile picture
          const maxWidth = 400;
          const maxHeight = 400;
          
          // Calculate new dimensions
          if (width > height) {
            if (width > maxWidth) {
              height = (height * maxWidth) / width;
              width = maxWidth;
            }
          } else {
            if (height > maxHeight) {
              width = (width * maxHeight) / height;
              height = maxHeight;
            }
          }
          
          canvas.width = width;
          canvas.height = height;
          
          const ctx = canvas.getContext('2d');
          ctx?.drawImage(img, 0, 0, width, height);
          
          // Convert to base64 with compression (0.8 quality)
          const compressedBase64 = canvas.toDataURL('image/jpeg', 0.8);
          resolve(compressedBase64);
        };
        img.onerror = reject;
        img.src = e.target?.result as string;
      };
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  };

  const handleSaveProfile = async () => {
    if (!username.trim() || !email.trim()) {
      alert('Username and email are required');
      return;
    }

    setSaving(true);

    try {
      const token = localStorage.getItem('token');
      
      // Get current user data first
      const currentUserResponse = await fetch('/api/auth/me', {
        headers: {
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        }
      });
      
      if (!currentUserResponse.ok) {
        throw new Error('Failed to get current user');
      }
      
      const currentUser = await currentUserResponse.json();
      
      // Update with all required fields
      const response = await fetch('/api/User/profile', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        },
        body: JSON.stringify({
          id: currentUser.id,
          name: currentUser.name,
          username: username,
          email: email,
          avatar: currentUser.avatar,
          bio: currentUser.bio,
          isOnline: currentUser.isOnline,
          lastSeen: currentUser.lastSeen
        })
      });

      if (!response.ok) {
        // Try to parse error message
        let errorMessage = 'Failed to update profile';
        try {
          const errorData = await response.json();
          errorMessage = errorData.message || errorMessage;
        } catch {
          // If JSON parsing fails, use status text
          errorMessage = response.statusText || errorMessage;
        }
        throw new Error(errorMessage);
      }

      const updatedUser = await response.json();
      
      // Update auth store
      if (token) {
        setAuth(updatedUser, token);
      }

      alert('Profile updated successfully!');
      onClose();
    } catch (error) {
      console.error('Error updating profile:', error);
      alert(error instanceof Error ? error.message : 'Failed to update profile');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">Edit Profile</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="space-y-4">
          {/* Profile Picture Upload */}
          <div className="flex flex-col items-center">
            <div className="relative">
              {previewImage || user?.avatar ? (
                <img 
                  src={previewImage || user?.avatar || ''} 
                  alt="Profile"
                  className="w-24 h-24 rounded-full object-cover"
                />
              ) : (
                <div className="w-24 h-24 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white text-3xl font-bold">
                  {user?.username.charAt(0) || 'U'}
                </div>
              )}
              <label 
                htmlFor="profile-picture-upload"
                className="absolute bottom-0 right-0 w-8 h-8 bg-cyan-500 rounded-full flex items-center justify-center cursor-pointer hover:bg-cyan-600 transition"
              >
                {uploading ? (
                  <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                ) : (
                  <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                )}
              </label>
              <input
                id="profile-picture-upload"
                type="file"
                accept="image/*"
                onChange={handleImageUpload}
                className="hidden"
                disabled={uploading}
              />
            </div>
            <p className="text-xs text-gray-500 mt-2">Click camera icon to upload</p>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Username</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-500"
              disabled={saving}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-500"
              disabled={saving}
            />
          </div>
        </div>

        <div className="flex gap-3 mt-6">
          <button
            onClick={onClose}
            className="flex-1 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
            disabled={saving}
          >
            Cancel
          </button>
          <button
            onClick={handleSaveProfile}
            className="flex-1 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            disabled={saving}
          >
            {saving ? (
              <>
                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                <span>Saving...</span>
              </>
            ) : (
              'Save'
            )}
          </button>
        </div>
      </div>
    </div>
  );
}

// Account Modal
function AccountModal({ onClose }: { onClose: () => void }) {
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">Account Settings</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="space-y-2">
          <SettingOption title="Privacy" subtitle="Last seen, profile photo, about" />
          <SettingOption title="Security" subtitle="Show security notifications" />
          <SettingOption title="Two-step verification" subtitle="Add extra security to your account" />
          <SettingOption title="Change number" subtitle="Change your phone number" />
          <SettingOption title="Request account info" subtitle="Request a report of your account info" />
          <SettingOption title="Delete my account" subtitle="Delete your account and erase your data" />
        </div>

        <button
          onClick={onClose}
          className="w-full mt-6 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition"
        >
          Close
        </button>
      </div>
    </div>
  );
}

// Chats Modal
function ChatsModal({ onClose }: { onClose: () => void }) {
  const [darkMode, setDarkMode] = useState(false);
  const [wallpaper, setWallpaper] = useState('default');

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">Chat Settings</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <h4 className="font-medium text-gray-900">Dark Mode</h4>
              <p className="text-xs text-gray-500">Enable dark theme</p>
            </div>
            <button
              onClick={() => setDarkMode(!darkMode)}
              className={`relative w-12 h-6 rounded-full transition ${
                darkMode ? 'bg-cyan-500' : 'bg-gray-300'
              }`}
            >
              <div
                className={`absolute top-1 left-1 w-4 h-4 bg-white rounded-full transition-transform ${
                  darkMode ? 'transform translate-x-6' : ''
                }`}
              />
            </button>
          </div>

          <div>
            <h4 className="font-medium text-gray-900 mb-2">Wallpaper</h4>
            <select
              value={wallpaper}
              onChange={(e) => setWallpaper(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-500"
            >
              <option value="default">Default</option>
              <option value="light">Light</option>
              <option value="dark">Dark</option>
              <option value="custom">Custom</option>
            </select>
          </div>

          <SettingOption title="Chat history" subtitle="Export chat history" />
          <SettingOption title="Archive all chats" subtitle="Archive all conversations" />
          <SettingOption title="Clear all chats" subtitle="Delete all messages" />
        </div>

        <button
          onClick={onClose}
          className="w-full mt-6 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition"
        >
          Close
        </button>
      </div>
    </div>
  );
}

// Notifications Modal
function NotificationsModal({ onClose }: { onClose: () => void }) {
  const [messageNotif, setMessageNotif] = useState(true);
  const [groupNotif, setGroupNotif] = useState(true);
  const [callNotif, setCallNotif] = useState(true);

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">Notification Settings</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="space-y-4">
          <ToggleSetting
            title="Message notifications"
            subtitle="Show notifications for new messages"
            value={messageNotif}
            onChange={setMessageNotif}
          />
          <ToggleSetting
            title="Group notifications"
            subtitle="Show notifications for group messages"
            value={groupNotif}
            onChange={setGroupNotif}
          />
          <ToggleSetting
            title="Call notifications"
            subtitle="Show notifications for incoming calls"
            value={callNotif}
            onChange={setCallNotif}
          />

          <div className="border-t border-gray-200 pt-4 mt-4">
            <SettingOption title="Notification tone" subtitle="Choose notification sound" />
            <SettingOption title="Vibrate" subtitle="Vibrate on notification" />
            <SettingOption title="Popup notification" subtitle="Show popup for new messages" />
          </div>
        </div>

        <button
          onClick={onClose}
          className="w-full mt-6 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition"
        >
          Close
        </button>
      </div>
    </div>
  );
}

// Storage Modal
function StorageModal({ onClose }: { onClose: () => void }) {
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">Storage and Data</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="space-y-2">
          <div className="p-4 bg-gray-50 rounded-lg mb-4">
            <div className="flex items-center justify-between mb-2">
              <span className="text-sm text-gray-600">Storage used</span>
              <span className="text-sm font-semibold text-gray-900">124 MB</span>
            </div>
            <div className="w-full h-2 bg-gray-200 rounded-full overflow-hidden">
              <div className="h-full bg-cyan-500" style={{ width: '35%' }}></div>
            </div>
          </div>

          <SettingOption title="Manage storage" subtitle="Free up space on your device" />
          <SettingOption title="Network usage" subtitle="View data usage statistics" />
          <SettingOption title="Auto-download media" subtitle="When using mobile data" />
          <SettingOption title="Auto-download media" subtitle="When connected on Wi-Fi" />
          <SettingOption title="Media quality" subtitle="Choose upload quality" />
        </div>

        <button
          onClick={onClose}
          className="w-full mt-6 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition"
        >
          Close
        </button>
      </div>
    </div>
  );
}

// Help Modal
function HelpModal({ onClose }: { onClose: () => void }) {
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold">Help</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="space-y-2">
          <SettingOption title="Help center" subtitle="Get answers to your questions" />
          <SettingOption title="Contact us" subtitle="Send us a message" />
          <SettingOption title="Terms and Privacy Policy" subtitle="Read our terms and policies" />
          <SettingOption title="App info" subtitle="Version 1.0.0" />
        </div>

        <div className="mt-6 p-4 bg-cyan-50 rounded-lg">
          <h4 className="font-semibold text-cyan-900 mb-2">About Talky</h4>
          <p className="text-sm text-cyan-700">
            Talky is a modern real-time messaging platform built with React and .NET 8.
            Enjoy secure, fast, and reliable communication with your friends and groups.
          </p>
        </div>

        <button
          onClick={onClose}
          className="w-full mt-6 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition"
        >
          Close
        </button>
      </div>
    </div>
  );
}

// Helper Components
function SettingOption({ title, subtitle }: { title: string; subtitle: string }) {
  return (
    <button className="w-full px-4 py-3 text-left hover:bg-gray-50 rounded-lg transition">
      <h4 className="font-medium text-gray-900 text-sm">{title}</h4>
      <p className="text-xs text-gray-500 mt-0.5">{subtitle}</p>
    </button>
  );
}

function ToggleSetting({ 
  title, 
  subtitle, 
  value, 
  onChange 
}: { 
  title: string; 
  subtitle: string; 
  value: boolean; 
  onChange: (value: boolean) => void;
}) {
  return (
    <div className="flex items-center justify-between py-2">
      <div className="flex-1">
        <h4 className="font-medium text-gray-900 text-sm">{title}</h4>
        <p className="text-xs text-gray-500 mt-0.5">{subtitle}</p>
      </div>
      <button
        onClick={() => onChange(!value)}
        className={`relative w-12 h-6 rounded-full transition ${
          value ? 'bg-cyan-500' : 'bg-gray-300'
        }`}
      >
        <div
          className={`absolute top-1 left-1 w-4 h-4 bg-white rounded-full transition-transform ${
            value ? 'transform translate-x-6' : ''
          }`}
        />
      </button>
    </div>
  );
}
