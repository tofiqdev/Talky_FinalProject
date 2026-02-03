// CEO email kontrolü için utility function
export const isCEO = (email: string): boolean => {
  const result = email === 'sixemirli99@gmail.com';
  console.log('CEO check:', { email, result }); // Debug log
  return result;
};

// CEO username kontrolü için utility function
export const isCEOByUsername = (username: string): boolean => {
  const result = username === 'Ourmine';
  console.log('CEO username check:', { username, result }); // Debug log
  return result;
};

// Birleşik CEO kontrolü (email veya username)
export const isCEOUser = (email?: string, username?: string): boolean => {
  return !!(email && isCEO(email)) || !!(username && isCEOByUsername(username));
};

// CEO badge component'i için props
export interface CEOBadgeProps {
  email: string;
  className?: string;
}

// CEO badge render function
export const renderCEOBadge = (email: string, className: string = '') => {
  console.log('renderCEOBadge called with:', { email, className }); // Debug log
  if (!isCEO(email)) return null;
  
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 ml-2 ${className}`}>
      CEO
    </span>
  );
};

// CEO badge render function by username
export const renderCEOBadgeByUsername = (username: string, className: string = '') => {
  console.log('renderCEOBadgeByUsername called with:', { username, className }); // Debug log
  if (!isCEOByUsername(username)) return null;
  
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 ml-2 ${className}`}>
      CEO
    </span>
  );
};

// Birleşik CEO badge render function (email veya username ile)
export const renderCEOBadgeUniversal = (email?: string, username?: string, className: string = '') => {
  if (!isCEOUser(email, username)) return null;
  
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 ml-2 ${className}`}>
      CEO
    </span>
  );
};