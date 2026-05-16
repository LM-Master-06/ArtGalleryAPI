import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Users, Plus, Pencil, Trash2, Search, X, MapPin } from 'lucide-react';
import type { Artist } from '../types';
import * as api from '../services/api';
import Modal from './Modal';

interface Toast { id: number; message: string; type: 'success' | 'error'; }

export default function ArtistsSection() {
  const [artists, setArtists] = useState<Artist[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Artist | null>(null);
  const [toasts, setToasts] = useState<Toast[]>([]);
  const [form, setForm] = useState({
    firstName: '', lastName: '', clanGroup: '', region: '',
    languageGroup: '', biography: '', birthDate: '', isDeceased: false,
  });

  const isLoggedIn = !!localStorage.getItem('token');

  const showToast = (message: string, type: 'success' | 'error' = 'success') => {
    const id = Date.now();
    setToasts(prev => [...prev, { id, message, type }]);
    setTimeout(() => setToasts(prev => prev.filter(t => t.id !== id)), 3000);
  };

  const fetchData = async () => {
    try { const data = await api.getArtists(); setArtists(data); }
    catch { showToast('Failed to load artists', 'error'); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchData(); }, []);

  const openCreate = () => {
    setEditing(null);
    setForm({ firstName: '', lastName: '', clanGroup: '', region: '', languageGroup: '', biography: '', birthDate: '', isDeceased: false });
    setModalOpen(true);
  };

  const openEdit = (artist: Artist) => {
    setEditing(artist);
    setForm({
      firstName: artist.firstName, lastName: artist.lastName, clanGroup: artist.clanGroup || '',
      region: artist.region || '', languageGroup: artist.languageGroup || '',
      biography: artist.biography || '', birthDate: artist.birthDate ? artist.birthDate.split('T')[0] : '',
      isDeceased: artist.isDeceased,
    });
    setModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const payload = {
        firstName: form.firstName, lastName: form.lastName, clanGroup: form.clanGroup || undefined,
        region: form.region || undefined, languageGroup: form.languageGroup || undefined,
        biography: form.biography || undefined, birthDate: form.birthDate || undefined, isDeceased: form.isDeceased,
        createdAt: new Date().toISOString(), artifactCount: 0,
      };
      if (editing) { await api.updateArtist(editing.id, payload); showToast('Artist updated'); }
      else { await api.createArtist(payload); showToast('Artist created'); }
      setModalOpen(false); fetchData();
    } catch { showToast('Operation failed', 'error'); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this artist?')) return;
    try { await api.deleteArtist(id); showToast('Artist deleted'); fetchData(); }
    catch { showToast('Failed to delete', 'error'); }
  };

  const filtered = artists.filter(a =>
    a.fullName.toLowerCase().includes(search.toLowerCase()) ||
    (a.region || '').toLowerCase().includes(search.toLowerCase()) ||
    (a.clanGroup || '').toLowerCase().includes(search.toLowerCase())
  );

  if (loading) return <div className="flex items-center justify-center h-96"><div className="w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-white">Artists</h2>
          <p className="text-zinc-400">Aboriginal artists in the gallery</p>
        </div>
        {isLoggedIn && (
          <motion.button onClick={openCreate} whileHover={{ scale: 1.03 }} whileTap={{ scale: 0.97 }}
            className="flex items-center gap-2 px-5 py-2.5 bg-amber-500 hover:bg-amber-400 text-black font-semibold rounded-xl transition-colors">
            <Plus className="w-4 h-4" /> Add Artist
          </motion.button>
        )}
      </div>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
        <input type="text" placeholder="Search artists..." value={search} onChange={e => setSearch(e.target.value)}
          className="w-full pl-10 pr-4 py-2.5 bg-zinc-900/50 border border-white/10 rounded-xl text-white placeholder-zinc-500 focus:outline-none focus:border-amber-500/50 transition-colors" />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <AnimatePresence mode="popLayout">
          {filtered.map((artist, i) => (
            <motion.div key={artist.id} layout initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.9 }} transition={{ duration: 0.3, delay: i * 0.05 }}
              className="group bg-zinc-900/50 border border-white/5 rounded-2xl p-5 hover:border-white/10 transition-colors">
              <div className="flex items-start justify-between mb-4">
                <div className="w-12 h-12 rounded-xl bg-blue-500/20 flex items-center justify-center">
                  <span className="text-lg font-bold text-blue-400">{artist.fullName[0]}</span>
                </div>
                {isLoggedIn && (
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button onClick={() => openEdit(artist)} className="w-8 h-8 rounded-lg bg-zinc-800 hover:bg-zinc-700 flex items-center justify-center text-zinc-400 hover:text-white transition-colors">
                      <Pencil className="w-3.5 h-3.5" />
                    </button>
                    <button onClick={() => handleDelete(artist.id)} className="w-8 h-8 rounded-lg bg-zinc-800 hover:bg-red-500/20 flex items-center justify-center text-zinc-400 hover:text-red-400 transition-colors">
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                )}
              </div>
              <h3 className="text-base font-semibold text-white mb-1">{artist.fullName}</h3>
              {artist.isDeceased && <span className="text-xs text-zinc-500 italic">Deceased</span>}
              <div className="mt-2 space-y-1">
                {artist.region && (
                  <div className="flex items-center gap-1.5 text-xs text-zinc-500">
                    <MapPin className="w-3 h-3" /> {artist.region}
                  </div>
                )}
                {artist.clanGroup && <div className="text-xs text-zinc-500">Clan: {artist.clanGroup}</div>}
                {artist.languageGroup && <div className="text-xs text-zinc-500">Language: {artist.languageGroup}</div>}
              </div>
              {artist.biography && <p className="mt-3 text-xs text-zinc-500 line-clamp-2">{artist.biography}</p>}
              <div className="mt-3 pt-3 border-t border-white/5 flex items-center justify-between">
                <span className="text-xs text-zinc-600">{artist.artifactCount} artifact{artist.artifactCount !== 1 ? 's' : ''}</span>
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      {filtered.length === 0 && (
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-16">
          <Users className="w-12 h-12 text-zinc-700 mx-auto mb-4" />
          <p className="text-zinc-500">No artists found</p>
        </motion.div>
      )}

      <Modal isOpen={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Artist' : 'Add Artist'}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">First Name *</label>
              <input required value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="First name" />
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Last Name *</label>
              <input required value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="Last name" />
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Region</label>
              <input value={form.region} onChange={e => setForm({ ...form, region: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="e.g. Northern Territory" />
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Clan Group</label>
              <input value={form.clanGroup} onChange={e => setForm({ ...form, clanGroup: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="Clan group" />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Language Group</label>
            <input value={form.languageGroup} onChange={e => setForm({ ...form, languageGroup: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="Language group" />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Date of Birth</label>
            <input type="date" value={form.birthDate} onChange={e => setForm({ ...form, birthDate: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors" />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Biography</label>
            <textarea rows={3} value={form.biography} onChange={e => setForm({ ...form, biography: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors resize-none" placeholder="Artist biography..." />
          </div>
          <div className="flex items-center gap-3">
            <input type="checkbox" id="isDeceased" checked={form.isDeceased} onChange={e => setForm({ ...form, isDeceased: e.target.checked })}
              className="w-4 h-4 rounded border-white/20 bg-zinc-800 text-amber-500" />
            <label htmlFor="isDeceased" className="text-sm text-zinc-400">Artist is deceased</label>
          </div>
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={() => setModalOpen(false)} className="flex-1 px-4 py-2.5 bg-zinc-800 hover:bg-zinc-700 text-zinc-300 font-medium rounded-xl transition-colors">Cancel</button>
            <button type="submit" className="flex-1 px-4 py-2.5 bg-amber-500 hover:bg-amber-400 text-black font-semibold rounded-xl transition-colors">{editing ? 'Update' : 'Create'}</button>
          </div>
        </form>
      </Modal>

      <div className="fixed bottom-6 right-6 z-[200] space-y-2">
        <AnimatePresence>
          {toasts.map(toast => (
            <motion.div key={toast.id} initial={{ opacity: 0, y: 20, scale: 0.9 }} animate={{ opacity: 1, y: 0, scale: 1 }} exit={{ opacity: 0, y: 20, scale: 0.9 }}
              className={`flex items-center gap-2 px-4 py-3 rounded-xl shadow-lg ${toast.type === 'success' ? 'bg-emerald-500/20 border border-emerald-500/30 text-emerald-400' : 'bg-red-500/20 border border-red-500/30 text-red-400'}`}>
              <span className="text-sm font-medium">{toast.message}</span>
              <button onClick={() => setToasts(prev => prev.filter(t => t.id !== toast.id))}><X className="w-3.5 h-3.5" /></button>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>
    </div>
  );
}
