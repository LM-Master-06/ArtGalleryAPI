import { motion } from 'framer-motion';
import { Image, Users, Tag, LayoutDashboard, LogIn, LogOut } from 'lucide-react';
import type { Section } from '../types';

interface NavbarProps {
  activeSection: Section;
  onSectionChange: (section: Section) => void;
  isLoggedIn: boolean;
  onLogout: () => void;
}

const navItems: { label: string; section: Section; icon: React.ElementType }[] = [
  { label: 'Dashboard', section: 'dashboard', icon: LayoutDashboard },
  { label: 'Artifacts', section: 'artifacts', icon: Image },
  { label: 'Artists', section: 'artists', icon: Users },
  { label: 'Art Types', section: 'artTypes', icon: Tag },
];

export default function Navbar({ activeSection, onSectionChange, isLoggedIn, onLogout }: NavbarProps) {
  return (
    <nav className="fixed top-0 left-0 right-0 z-50 border-b border-white/5 bg-[#0a0a0a]/80 backdrop-blur-xl">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-lg bg-amber-500/20 border border-amber-500/30 flex items-center justify-center">
              <Image className="w-4 h-4 text-amber-400" />
            </div>
            <span className="text-white font-semibold text-sm">ArtGallery</span>
          </div>

          <div className="flex items-center gap-1">
            {navItems.map(({ label, section, icon: Icon }) => (
              <motion.button
                key={section}
                onClick={() => onSectionChange(section)}
                whileTap={{ scale: 0.97 }}
                className={`flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                  activeSection === section
                    ? 'bg-white/10 text-white'
                    : 'text-zinc-400 hover:text-white hover:bg-white/5'
                }`}
              >
                <Icon className="w-4 h-4" />
                <span className="hidden sm:inline">{label}</span>
              </motion.button>
            ))}
          </div>

          <div>
            {isLoggedIn ? (
              <motion.button
                onClick={onLogout}
                whileTap={{ scale: 0.97 }}
                className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium text-zinc-400 hover:text-white hover:bg-white/5 transition-colors"
              >
                <LogOut className="w-4 h-4" />
                <span className="hidden sm:inline">Logout</span>
              </motion.button>
            ) : (
              <motion.button
                onClick={() => onSectionChange('auth')}
                whileTap={{ scale: 0.97 }}
                className={`flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                  activeSection === 'auth'
                    ? 'bg-amber-500/20 text-amber-400'
                    : 'text-zinc-400 hover:text-white hover:bg-white/5'
                }`}
              >
                <LogIn className="w-4 h-4" />
                <span className="hidden sm:inline">Login</span>
              </motion.button>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
