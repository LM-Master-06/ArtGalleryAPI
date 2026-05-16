import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Tag, Plus, Pencil, Trash2, Search, X, MapPin, Wrench } from 'lucide-react';
import type { ArtType } from '../types';
import * as api from '../services/api';
import Modal from './Modal';

interface Toast { id: number; message: string; type: 'success' | 'error'; }

export default function ArtTypesSection() {
  const [artTypes, setArtTypes] = useState<ArtType[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ArtType | null>(null);
  const [toasts, setToasts] = useState<Toast[]>([]);
  const [form, setForm] = useState({ name: '', description: '', region: '', technique: '' });

  const isLoggedIn = !!localStorage.getItem('token');

  const showToast = (message: string, type: 'success' | 'error' = 'success') => {
    const id = Date.now();
    setToasts(prev => [...prev, { id, message, type }]);
    setTimeout(() => setToasts(prev => prev.filter(t => t.id !== id)), 3000);
  };

  const fetchData = async () => {
    try { const data = await api.getArtTypes(); setArtTypes(data); }
    catch { showToast('Failed to load art types', 'error'); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchData(); }, []);

  const openCreate = () => {
    setEditing(null);
    setForm({ name: '', description: '', region: '', technique: '' });
    setModalOpen(true);
  };

  const openEdit = (artType: ArtType) => {
    setEditing(artType);
    setForm({ name: artType.name, description: artType.description, region: artType.region, technique: artType.technique });
    setModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editing) { await api.updateArtType(editing.id, form); showToast('Art type updated'); }
      else { await api.createArtType(form); showToast('Art type created'); }
      setModalOpen(false); fetchData();
    } catch { showToast('Operation failed', 'error'); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this art type?')) return;
    try { await api.deleteArtType(id); showToast('Art type deleted'); fetchData(); }
    catch { showToast('Failed to delete', 'error'); }
  };

  const filtered = artTypes.filter(t =>
    t.name.toLowerCase().includes(search.toLowerCase()) ||
    t.region.toLowerCase().includes(search.toLowerCase()) ||
    t.technique.toLowerCase().includes(search.toLowerCase())
  );

  if (loading) return <div className="flex items-center justify-center h-96"><div className="w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-white">Art Types</h2>
          <p className="text-zinc-400">Aboriginal art categories and techniques</p>
        </div>
        {isLoggedIn && (
          <motion.button onClick={openCreate} whileHover={{ scale: 1.03 }} whileTap={{ scale: 0.97 }}
            className="flex items-center gap-2 px-5 py-2.5 bg-amber-500 hover:bg-amber-400 text-black font-semibold rounded-xl transition-colors">
            <Plus className="w-4 h-4" /> Add Art Type
          </motion.button>
        )}
      </div>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
        <input type="text" placeholder="Search art types..." value={search} onChange={e => setSearch(e.target.value)}
          className="w-full pl-10 pr-4 py-2.5 bg-zinc-900/50 border border-white/10 rounded-xl text-white placeholder-zinc-500 focus:outline-none focus:border-amber-500/50 transition-colors" />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <AnimatePresence mode="popLayout">
          {filtered.map((artType, i) => (
            <motion.div key={artType.id} layout initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.9 }} transition={{ duration: 0.3, delay: i * 0.05 }}
              className="group bg-zinc-900/50 border border-white/5 rounded-2xl p-5 hover:border-white/10 transition-colors">
              <div className="flex items-start justify-between mb-4">
                <div className="w-10 h-10 rounded-xl bg-purple-500/20 flex items-center justify-center">
                  <Tag className="w-5 h-5 text-purple-400" />
                </div>
                {isLoggedIn && (
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button onClick={() => openEdit(artType)} className="w-8 h-8 rounded-lg bg-zinc-800 hover:bg-zinc-700 flex items-center justify-center text-zinc-400 hover:text-white transition-colors">
                      <Pencil className="w-3.5 h-3.5" />
                    </button>
                    <button onClick={() => handleDelete(artType.id)} className="w-8 h-8 rounded-lg bg-zinc-800 hover:bg-red-500/20 flex items-center justify-center text-zinc-400 hover:text-red-400 transition-colors">
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                )}
              </div>
              <h3 className="text-base font-semibold text-white mb-2">{artType.name}</h3>
              <p className="text-sm text-zinc-500 line-clamp-2 mb-3">{artType.description}</p>
              <div className="space-y-1.5">
                <div className="flex items-center gap-1.5 text-xs text-zinc-500">
                  <MapPin className="w-3 h-3 text-purple-400" /> {artType.region}
                </div>
                <div className="flex items-center gap-1.5 text-xs text-zinc-500">
                  <Wrench className="w-3 h-3 text-amber-400" /> {artType.technique}
                </div>
              </div>
              <div className="mt-3 pt-3 border-t border-white/5">
                <span className="text-xs text-zinc-600">{artType.artifactCount} artifact{artType.artifactCount !== 1 ? 's' : ''}</span>
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      {filtered.length === 0 && (
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-16">
          <Tag className="w-12 h-12 text-zinc-700 mx-auto mb-4" />
          <p className="text-zinc-500">No art types found</p>
        </motion.div>
      )}

      <Modal isOpen={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Art Type' : 'Add Art Type'}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Name *</label>
            <input required value={form.name} onChange={e => setForm({ ...form, name: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="e.g. Dot Painting" />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Region *</label>
            <input required value={form.region} onChange={e => setForm({ ...form, region: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="e.g. Central Desert" />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Technique *</label>
            <input required value={form.technique} onChange={e => setForm({ ...form, technique: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="e.g. Acrylic dots on canvas" />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Description *</label>
            <textarea required rows={3} value={form.description} onChange={e => setForm({ ...form, description: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors resize-none" placeholder="Describe this art type..." />
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
