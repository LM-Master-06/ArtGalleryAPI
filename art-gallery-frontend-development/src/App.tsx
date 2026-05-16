import { useState, useRef, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import type { Section } from './types';
import Navbar from './components/Navbar';
import Hero from './components/Hero';
import Dashboard from './components/Dashboard';
import ArtifactsSection from './components/ArtifactsSection';
import ArtistsSection from './components/ArtistsSection';
import ArtTypesSection from './components/ArtTypesSection';
import AuthSection from './components/AuthSection';

export default function App() {
  const [activeSection, setActiveSection] = useState<Section>('dashboard');
  const [showHero, setShowHero] = useState(true);
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
  const contentRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleStorage = () => setIsLoggedIn(!!localStorage.getItem('token'));
    window.addEventListener('storage', handleStorage);
    return () => window.removeEventListener('storage', handleStorage);
  }, []);

  const handleExplore = () => {
    setShowHero(false);
    setTimeout(() => contentRef.current?.scrollIntoView({ behavior: 'smooth' }), 100);
  };

  const handleSectionChange = (section: Section) => {
    setShowHero(false);
    setActiveSection(section);
    setTimeout(() => contentRef.current?.scrollIntoView({ behavior: 'smooth' }), 100);
  };

  const handleLogin = () => {
    setIsLoggedIn(true);
    setActiveSection('dashboard');
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    setIsLoggedIn(false);
    setActiveSection('dashboard');
  };

  const renderSection = () => {
    switch (activeSection) {
      case 'dashboard': return <Dashboard />;
      case 'artifacts': return <ArtifactsSection />;
      case 'artists': return <ArtistsSection />;
      case 'artTypes': return <ArtTypesSection />;
      case 'auth': return <AuthSection onLogin={handleLogin} />;
      default: return <Dashboard />;
    }
  };

  return (
    <div className="min-h-screen bg-[#0a0a0a]">
      <Navbar
        activeSection={activeSection}
        onSectionChange={handleSectionChange}
        isLoggedIn={isLoggedIn}
        onLogout={handleLogout}
      />

      <AnimatePresence mode="wait">
        {showHero && (
          <motion.div
            key="hero"
            initial={{ opacity: 1 }}
            exit={{ opacity: 0, height: 0 }}
            transition={{ duration: 0.5 }}
          >
            <Hero onExplore={handleExplore} />
          </motion.div>
        )}
      </AnimatePresence>

      <main
        ref={contentRef}
        className={`max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 ${showHero ? 'pt-16' : 'pt-24'} pb-16`}
      >
        <AnimatePresence mode="wait">
          <motion.div
            key={activeSection}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.3 }}
          >
            {renderSection()}
          </motion.div>
        </AnimatePresence>
      </main>

      <footer className="border-t border-white/5 py-8">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <p className="text-sm text-zinc-600">
            Art Gallery Management System &middot; Connected to localhost:5027
          </p>
        </div>
      </footer>
    </div>
  );
}
