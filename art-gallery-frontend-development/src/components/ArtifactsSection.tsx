import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Image, Plus, Pencil, Trash2, Search, X, Filter, DollarSign, CheckCircle, XCircle } from 'lucide-react';
import type { Artifact, Artist, ArtType } from '../types';
import * as api from '../services/api';
import Modal from './Modal';

interface Toast { id: number; message: string; type: 'success' | 'error'; }

export default function ArtifactsSection() {
  const [artifacts, setArtifacts] = useState<Artifact[]>([]);
  const [artists, setArtists] = useState<Artist[]>([]);
  const [artTypes, setArtTypes] = useState<ArtType[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [filterType, setFilterType] = useState<number | 'all'>('all');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Artifact | null>(null);
  const [toasts, setToasts] = useState<Toast[]>([]);
  const [form, setForm] = useState({
    title: '', medium: '', yearCreated: new Date().getFullYear(),
    description: '', story: '', dimensions: '', imageUrl: '',
    price: '', isAvailable: true, artistId: '', artTypeId: '',
  });

  const isLoggedIn = !!localStorage.getItem('token');

  const showToast = (message: string, type: 'success' | 'error' = 'success') => {
    const id = Date.now();
    setToasts(prev => [...prev, { id, message, type }]);
    setTimeout(() => setToasts(prev => prev.filter(t => t.id !== id)), 3000);
  };

  const fetchData = async () => {
    try {
      const [a, ar, at] = await Promise.all([api.getArtifacts(), api.getArtists(), api.getArtTypes()]);
      setArtifacts(a);
      setArtists(ar);
      setArtTypes(at);
    } catch { showToast('Failed to load data', 'error'); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchData(); }, []);

  const openCreate = () => {
    setEditing(null);
    setForm({ title: '', medium: '', yearCreated: new Date().getFullYear(), description: '', story: '', dimensions: '', imageUrl: '', price: '', isAvailable: true, artistId: '', artTypeId: '' });
    setModalOpen(true);
  };

  const openEdit = (artifact: Artifact) => {
    setEditing(artifact);
    setForm({
      title: artifact.title, medium: artifact.medium || '', yearCreated: artifact.yearCreated || new Date().getFullYear(),
      description: artifact.description || '', story: artifact.story || '', dimensions: artifact.dimensions || '',
      imageUrl: artifact.imageUrl || '', price: artifact.price?.toString() || '', isAvailable: artifact.isAvailable,
      artistId: artifact.artist?.id.toString() || '', artTypeId: artifact.artType?.id.toString() || '',
    });
    setModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.artistId || !form.artTypeId) { showToast('Artist and Art Type are required', 'error'); return; }
    try {
      const payload = {
        title: form.title, medium: form.medium, yearCreated: Number(form.yearCreated),
        description: form.description, story: form.story, dimensions: form.dimensions,
        imageUrl: form.imageUrl || undefined, price: form.price ? Number(form.price) : undefined,
        isAvailable: form.isAvailable, artistId: Number(form.artistId), artTypeId: Number(form.artTypeId),
      };
      if (editing) {
        await api.updateArtifact(editing.id, payload);
        showToast('Artifact updated successfully');
      } else {
        await api.createArtifact(payload);
        showToast('Artifact created successfully');
      }
      setModalOpen(false);
      fetchData();
    } catch { showToast('Operation failed', 'error'); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this artifact?')) return;
    try { await api.deleteArtifact(id); showToast('Artifact deleted'); fetchData(); }
    catch { showToast('Failed to delete', 'error'); }
  };

  const filtered = artifacts.filter(a => {
    const matchesSearch = a.title.toLowerCase().includes(search.toLowerCase()) || (a.medium || '').toLowerCase().includes(search.toLowerCase());
    const matchesType = filterType === 'all' || a.artType?.id === filterType;
    return matchesSearch && matchesType;
  });

  if (loading) return <div className="flex items-center justify-center h-96"><div className="w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-white">Artifacts</h2>
          <p className="text-zinc-400">Aboriginal art collection</p>
        </div>
        {isLoggedIn && (
          <motion.button onClick={openCreate} whileHover={{ scale: 1.03 }} whileTap={{ scale: 0.97 }}
            className="flex items-center gap-2 px-5 py-2.5 bg-amber-500 hover:bg-amber-400 text-black font-semibold rounded-xl transition-colors">
            <Plus className="w-4 h-4" /> Add Artifact
          </motion.button>
        )}
      </div>

      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
          <input type="text" placeholder="Search artifacts..." value={search} onChange={e => setSearch(e.target.value)}
            className="w-full pl-10 pr-4 py-2.5 bg-zinc-900/50 border border-white/10 rounded-xl text-white placeholder-zinc-500 focus:outline-none focus:border-amber-500/50 transition-colors" />
        </div>
        <div className="relative">
          <Filter className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
          <select value={filterType} onChange={e => setFilterType(e.target.value === 'all' ? 'all' : Number(e.target.value))}
            className="pl-10 pr-8 py-2.5 bg-zinc-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors appearance-none cursor-pointer">
            <option value="all">All Art Types</option>
            {artTypes.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <AnimatePresence mode="popLayout">
          {filtered.map((artifact, i) => (
            <motion.div key={artifact.id} layout initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.9 }} transition={{ duration: 0.3, delay: i * 0.05 }}
              className="group bg-zinc-900/50 border border-white/5 rounded-2xl overflow-hidden hover:border-white/10 transition-colors">
              <div className="h-40 bg-amber-500/5 border-b border-white/5 flex items-center justify-center relative overflow-hidden">
                {artifact.imageUrl ? (
                  <img src={artifact.imageUrl} alt={artifact.title} className="w-full h-full object-cover" />
                ) : (
                  <>
                    <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_rgba(217,119,6,0.1),_transparent_70%)]" />
                    <Image className="w-12 h-12 text-amber-500/30 relative z-10" />
                  </>
                )}
                {isLoggedIn && (
                  <div className="absolute top-3 right-3 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button onClick={() => openEdit(artifact)} className="w-8 h-8 rounded-lg bg-zinc-800/80 hover:bg-zinc-700 flex items-center justify-center text-zinc-300 hover:text-white transition-colors">
                      <Pencil className="w-3.5 h-3.5" />
                    </button>
                    <button onClick={() => handleDelete(artifact.id)} className="w-8 h-8 rounded-lg bg-zinc-800/80 hover:bg-red-500/20 flex items-center justify-center text-zinc-300 hover:text-red-400 transition-colors">
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                )}
                <div className="absolute bottom-3 left-3">
                  {artifact.isAvailable
                    ? <span className="flex items-center gap-1 px-2 py-0.5 rounded-full bg-emerald-500/20 text-emerald-400 text-xs"><CheckCircle className="w-3 h-3" /> Available</span>
                    : <span className="flex items-center gap-1 px-2 py-0.5 rounded-full bg-red-500/20 text-red-400 text-xs"><XCircle className="w-3 h-3" /> Sold</span>
                  }
                </div>
              </div>
              <div className="p-5">
                <h3 className="text-base font-semibold text-white mb-1 truncate">{artifact.title}</h3>
                <p className="text-sm text-zinc-500 mb-3 line-clamp-2">{artifact.description}</p>
                <div className="flex items-center gap-2 flex-wrap">
                  {artifact.medium && <span className="px-2.5 py-1 rounded-md bg-amber-500/10 text-amber-400 text-xs font-medium">{artifact.medium}</span>}
                  {artifact.yearCreated && <span className="px-2.5 py-1 rounded-md bg-zinc-800 text-zinc-400 text-xs">{artifact.yearCreated}</span>}
                  {artifact.artType && <span className="px-2.5 py-1 rounded-md bg-purple-500/10 text-purple-400 text-xs">{artifact.artType.name}</span>}
                  {artifact.price != null && (
                    <span className="flex items-center gap-1 px-2.5 py-1 rounded-md bg-emerald-500/10 text-emerald-400 text-xs">
                      <DollarSign className="w-3 h-3" />{artifact.price.toLocaleString()}
                    </span>
                  )}
                </div>
                {artifact.artist && (
                  <div className="mt-3 pt-3 border-t border-white/5 flex items-center gap-2">
                    <div className="w-6 h-6 rounded-full bg-blue-500/20 flex items-center justify-center">
                      <span className="text-xs text-blue-400 font-medium">{artifact.artist.fullName[0]}</span>
                    </div>
                    <span className="text-xs text-zinc-400">{artifact.artist.fullName}</span>
                    {artifact.artist.region && <span className="text-xs text-zinc-600">&middot; {artifact.artist.region}</span>}
                  </div>
                )}
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      {filtered.length === 0 && (
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-16">
          <Image className="w-12 h-12 text-zinc-700 mx-auto mb-4" />
          <p className="text-zinc-500">No artifacts found</p>
        </motion.div>
      )}

      <Modal isOpen={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Artifact' : 'Add Artifact'}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Title *</label>
            <input required value={form.title} onChange={e => setForm({ ...form, title: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
              placeholder="Enter artifact title" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Medium</label>
              <input value={form.medium} onChange={e => setForm({ ...form, medium: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
                placeholder="e.g. Ochre on bark" />
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Year Created</label>
              <input type="number" value={form.yearCreated} onChange={e => setForm({ ...form, yearCreated: Number(e.target.value) })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" />
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Price ($)</label>
              <input type="number" step="0.01" value={form.price} onChange={e => setForm({ ...form, price: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
                placeholder="0.00" />
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Dimensions</label>
              <input value={form.dimensions} onChange={e => setForm({ ...form, dimensions: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
                placeholder="e.g. 60x40cm" />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Description</label>
            <textarea rows={2} value={form.description} onChange={e => setForm({ ...form, description: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors resize-none"
              placeholder="Brief description..." />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Story / Cultural Context</label>
            <textarea rows={2} value={form.story} onChange={e => setForm({ ...form, story: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors resize-none"
              placeholder="Cultural story or context..." />
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Image URL</label>
            <input value={form.imageUrl} onChange={e => setForm({ ...form, imageUrl: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
              placeholder="https://..." />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Artist *</label>
              <select required value={form.artistId} onChange={e => setForm({ ...form, artistId: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors appearance-none cursor-pointer">
                <option value="">Select artist</option>
                {artists.map(a => <option key={a.id} value={a.id}>{a.fullName}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Art Type *</label>
              <select required value={form.artTypeId} onChange={e => setForm({ ...form, artTypeId: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors appearance-none cursor-pointer">
                <option value="">Select art type</option>
                {artTypes.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
              </select>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <input type="checkbox" id="isAvailable" checked={form.isAvailable} onChange={e => setForm({ ...form, isAvailable: e.target.checked })}
              className="w-4 h-4 rounded border-white/20 bg-zinc-800 text-amber-500" />
            <label htmlFor="isAvailable" className="text-sm text-zinc-400">Available for purchase</label>
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
