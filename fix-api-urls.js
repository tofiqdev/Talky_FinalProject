const fs = require('fs');
const path = require('path');

const files = [
  'src/components/story/CreateStoryModal.tsx',
  'src/components/story/ViewStoryModal.tsx',
  'src/components/sidebar/ChatsTab.tsx',
  'src/components/group/CreateGroupModal.tsx',
  'src/components/group/GroupDetailsModal.tsx',
  'src/components/chat/ChatWindow.tsx',
  'src/components/sidebar/PeopleTab.tsx'
];

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  
  // Add import if not exists
  if (!content.includes('API_BASE_URL')) {
    const importLine = "import { API_BASE_URL } from '../../services/apiService';\n";
    const firstImportIndex = content.indexOf('import');
    if (firstImportIndex !== -1) {
      const endOfImports = content.indexOf('\n\n', firstImportIndex);
      content = content.slice(0, endOfImports) + '\n' + importLine + content.slice(endOfImports);
    }
  }
  
  // Replace all /api/ with ${API_BASE_URL}/
  content = content.replace(/fetch\(['"]\/api\//g, 'fetch(`${API_BASE_URL}/');
  content = content.replace(/\)$/gm, ')');
  
  fs.writeFileSync(file, content);
  console.log(`Fixed: ${file}`);
});

console.log('Done!');
