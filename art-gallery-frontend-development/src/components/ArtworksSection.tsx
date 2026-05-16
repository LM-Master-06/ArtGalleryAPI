import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Image, Plus, Pencil, Trash2, Search, X, Filter } from 'lucide-react';
import type { Artwork, Artist, Category } from '../types';
import * as api from '../services/api';
import Modal from './Modal';

interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error';
}

export default function ArtworksSection() {
  const [artworks, setArtworks] = useState<Artwork[]>([]);
  const [artists, setArtists] = useState<Artist[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [filterCategory, setFilterCategory] = useState<number | 'all'>('all');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Artwork | null>(null);
  const [toasts, setToasts] = useState<Toast[]>([]);
  const [form, setForm] = useState({
    title: '',
    medium: '',
    year: new Date().getFullYear(),
    description: '',
    artistId: '',
    categoryId: '',
  });

  const showToast = (message: string, type: 'success' | 'error' = 'success') => {
    const id = Date.now();
    setToasts(prev => [...prev, { id, message, type }]);
    setTimeout(() => setToasts(prev => prev.filter(t => t.id !== id)), 3000);
  };

  const fetchData = async () => {
    try {
      const [a, ar, c] = await Promise.all([
        api.getArtworks(),
        api.getArtists(),
        api.getCategories(),
      ]);
      setArtworks(a);
      setArtists(ar);
      setCategories(c);
    } catch (err) {
      showToast('Failed to load artworks', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const openCreate = () => {
    setEditing(null);
    setForm({ title: '', medium: '', year: new Date().getFullYear(), description: '', artistId: '', categoryId: '' });
    setModalOpen(true);
  };

  const openEdit = (artwork: Artwork) => {
    setEditing(artwork);
    setForm({
      title: artwork.title,
      medium: artwork.medium,
      year: artwork.year,
      description: artwork.description,
      artistId: artwork.artistId?.toString() || '',
      categoryId: artwork.categoryId?.toString() || '',
    });
    setModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const payload = {
        ...form,
        year: Number(form.year),
        artistId: form.artistId ? Number(form.artistId) : undefined,
        categoryId: form.categoryId ? Number(form.categoryId) : undefined,
      };
      if (editing) {
        await api.updateArtwork(editing.id, payload);
        showToast('Artwork updated successfully');
      } else {
        await api.createArtwork(payload);
        showToast('Artwork created successfully');
      }
      setModalOpen(false);
      fetchData();
    } catch (err) {
      showToast('Operation failed', 'error');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this artwork?')) return;
    try {
      await api.deleteArtwork(id);
      showToast('Artwork deleted');
      fetchData();
    } catch (err) {
      showToast('Failed to delete artwork', 'error');
    }
  };

  const filtered = artworks.filter(a => {
    const matchesSearch = a.title.toLowerCase().includes(search.toLowerCase()) ||
      a.medium.toLowerCase().includes(search.toLowerCase());
    const matchesCategory = filterCategory === 'all' || a.categoryId === filterCategory;
    return matchesSearch && matchesCategory;
  });

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-white">Artworks</h2>
          <p className="text-zinc-400">Manage your gallery collection</p>
        </div>
        <motion.button
          onClick={openCreate}
          whileHover={{ scale: 1.03 }}
          whileTap={{ scale: 0.97 }}
          className="flex items-center gap-2 px-5 py-2.5 bg-amber-500 hover:bg-amber-400 text-black font-semibold rounded-xl transition-colors"
        >
          <Plus className="w-4 h-4" />
          Add Artwork
        </motion.button>
      </div>

      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
          <input
            type="text"
            placeholder="Search artworks..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            className="w-full pl-10 pr-4 py-2.5 bg-zinc-900/50 border border-white/10 rounded-xl text-white placeholder-zinc-500 focus:outline-none focus:border-amber-500/50 transition-colors"
          />
        </div>
        <div className="relative">
          <Filter className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
          <select
            value={filterCategory}
            onChange={e => setFilterCategory(e.target.value === 'all' ? 'all' : Number(e.target.value))}
            className="pl-10 pr-8 py-2.5 bg-zinc-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors appearance-none cursor-pointer"
          >
            <option value="all">All Categories</option>
            {categories.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <AnimatePresence mode="popLayout">
          {filtered.map((artwork, i) => (
            <motion.div
              key={artwork.id}
              layout
              initial={{ opacity: 0, scale: 0.9 }}
              animate={{ opacity: 1, scale: 1 }}
              exit={{ opacity: 0, scale: 0.9 }}
              transition={{ duration: 0.3, delay: i * 0.05 }}
              className="group bg-zinc-900/50 border border-white/5 rounded-2xl overflow-hidden hover:border-white/10 transition-colors"
            >
              <div className="h-40 bg-amber-500/5 border-b border-white/5 flex items-center justify-center relative overflow-hidden">
                <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_rgba(217,119,6,0.1),_transparent_70%)]" />
                <Image className="w-12 h-12 text-amber-500/30 relative z-10" />
                <div className="absolute top-3 right-3 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button
                    onClick={() => openEdit(artwork)}
                    className="w-8 h-8 rounded-lg bg-zinc-800/80 hover:bg-zinc-700 flex items-center justify-center text-zinc-300 hover:text-white transition-colors"
                  >
                    <Pencil className="w-3.5 h-3.5" />
                  </button>
                  <button
                    onClick={() => handleDelete(artwork.id)}
                    className="w-8 h-8 rounded-lg bg-zinc-800/80 hover:bg-red-500/20 flex items-center justify-center text-zinc-300 hover:text-red-400 transition-colors"
                  >
                    <Trash2 className="w-3.5 h-3.5" />
                  </button>
                </div>
              </div>
              <div className="p-5">
                <h3 className="text-base font-semibold text-white mb-1 truncate">{artwork.title}</h3>
                <p className="text-sm text-zinc-500 mb-3 line-clamp-2">{artwork.description}</p>
                <div className="flex items-center gap-2 flex-wrap">
                  <span className="px-2.5 py-1 rounded-md bg-amber-500/10 text-amber-400 text-xs font-medium">
                    {artwork.medium}
                  </span>
                  <span className="px-2.5 py-1 rounded-md bg-zinc-800 text-zinc-400 text-xs">
                    {artwork.year}
                  </span>
                  {artwork.category && (
                    <span className="px-2.5 py-1 rounded-md bg-purple-500/10 text-purple-400 text-xs">
                      {artwork.category.name}
                    </span>
                  )}
                </div>
                {artwork.artist && (
                  <div className="mt-3 pt-3 border-t border-white/5 flex items-center gap-2">
                    <div className="w-6 h-6 rounded-full bg-blue-500/20 flex items-center justify-center">
                      <span className="text-xs text-blue-400 font-medium">{artwork.artist.name[0]}</span>
                    </div>
                    <span className="text-xs text-zinc-400">{artwork.artist.name}</span>
                  </div>
                )}
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      {filtered.length === 0 && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          className="text-center py-16"
        >
          <Image className="w-12 h-12 text-zinc-700 mx-auto mb-4" />
          <p className="text-zinc-500">No artworks found</p>
        </motion.div>
      )}

      <Modal isOpen={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Artwork' : 'Add Artwork'}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Title</label>
            <input
              required
              value={form.title}
              onChange={e => setForm({ ...form, title: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
              placeholder="Enter artwork title"
            />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Medium</label>
              <input
                required
                value={form.medium}
                onChange={e => setForm({ ...form, medium: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
                placeholder="e.g. Oil on canvas"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Year</label>
              <input
                required
                type="number"
                value={form.year}
                onChange={e => setForm({ ...form, year: Number(e.target.value) })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-zinc-400 mb-1.5">Description</label>
            <textarea
              required
              rows={3}
              value={form.description}
              onChange={e => setForm({ ...form, description: e.target.value })}
              className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors resize-none"
              placeholder="Describe the artwork..."
            />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Artist</label>
              <select
                value={form.artistId}
                onChange={e => setForm({ ...form, artistId: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors appearance-none cursor-pointer"
              >
                <option value="">Select artist</option>
                {artists.map(a => (
                  <option key={a.id} value={a.id}>{a.name}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Category</label>
              <select
                value={form.categoryId}
                onChange={e => setForm({ ...form, categoryId: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white focus:outline-none focus:border-amber-500/50 transition-colors appearance-none cursor-pointer"
              >
                <option value="">Select category</option>
                {categories.map(c => (
                  <option key={c.id} value={c.id}>{c.name}</option>
                ))}
              </select>
            </div>
          </div>
          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={() => setModalOpen(false)}
              className="flex-1 px-4 py-2.5 bg-zinc-800 hover:bg-zinc-700 text-zinc-300 font-medium rounded-xl transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="flex-1 px-4 py-2.5 bg-amber-500 hover:bg-amber-400 text-black font-semibold rounded-xl transition-colors"
            >
              {editing ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </Modal>

      {/* Toasts */}
      <div className="fixed bottom-6 right-6 z-[200] space-y-2">
        <AnimatePresence>
          {toasts.map(toast => (
            <motion.div
              key={toast.id}
              initial={{ opacity: 0, y: 20, scale: 0.9 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: 20, scale: 0.9 }}
              className={`flex items-center gap-2 px-4 py-3 rounded-xl shadow-lg ${
                toast.type === 'success' ? 'bg-emerald-500/20 border border-emerald-500/30 text-emerald-400' : 'bg-red-500/20 border border-red-500/30 text-red-400'
              }`}
            >
              <span className="text-sm font-medium">{toast.message}</span>
              <button onClick={() => setToasts(prev => prev.filter(t => t.id !== toast.id))}>
                <X className="w-3.5 h-3.5" />
              </button>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>
    </div>
  );
}
