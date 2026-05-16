import { useState } from 'react';
import { motion } from 'framer-motion';
import { LogIn, UserPlus, Eye, EyeOff } from 'lucide-react';
import * as api from '../services/api';

interface AuthSectionProps {
  onLogin: () => void;
}

export default function AuthSection({ onLogin }: AuthSectionProps) {
  const [mode, setMode] = useState<'login' | 'register'>('login');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [form, setForm] = useState({ email: '', password: '', firstName: '', lastName: '' });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      let result;
      if (mode === 'login') {
        result = await api.login(form.email, form.password);
      } else {
        result = await api.register(form.email, form.password, form.firstName, form.lastName);
      }
      localStorage.setItem('token', result.token);
      window.dispatchEvent(new Event('storage'));
      onLogin();
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr?.response?.data?.message || 'Authentication failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-[60vh]">
      <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5 }}
        className="w-full max-w-md">
        <div className="bg-zinc-900/50 border border-white/5 rounded-2xl p-8">
          <div className="flex items-center gap-3 mb-8">
            <div className="w-10 h-10 rounded-xl bg-amber-500/20 flex items-center justify-center">
              {mode === 'login' ? <LogIn className="w-5 h-5 text-amber-400" /> : <UserPlus className="w-5 h-5 text-amber-400" />}
            </div>
            <div>
              <h2 className="text-xl font-bold text-white">{mode === 'login' ? 'Sign In' : 'Create Account'}</h2>
              <p className="text-sm text-zinc-500">{mode === 'login' ? 'Access the gallery management system' : 'Register a new account'}</p>
            </div>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            {mode === 'register' && (
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-zinc-400 mb-1.5">First Name</label>
                  <input required value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })}
                    className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="First name" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-zinc-400 mb-1.5">Last Name</label>
                  <input required value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })}
                    className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="Last name" />
                </div>
              </div>
            )}
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Email</label>
              <input required type="email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })}
                className="w-full px-4 py-2.5 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="you@example.com" />
            </div>
            <div>
              <label className="block text-sm font-medium text-zinc-400 mb-1.5">Password</label>
              <div className="relative">
                <input required type={showPassword ? 'text' : 'password'} value={form.password} onChange={e => setForm({ ...form, password: e.target.value })}
                  className="w-full px-4 py-2.5 pr-10 bg-zinc-800 border border-white/10 rounded-xl text-white placeholder-zinc-600 focus:outline-none focus:border-amber-500/50 transition-colors" placeholder="Password" />
                <button type="button" onClick={() => setShowPassword(!showPassword)} className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-500 hover:text-zinc-300 transition-colors">
                  {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                </button>
              </div>
            </div>

            {error && (
              <div className="px-4 py-3 rounded-xl bg-red-500/10 border border-red-500/20 text-red-400 text-sm">{error}</div>
            )}

            <motion.button type="submit" disabled={loading} whileHover={{ scale: 1.02 }} whileTap={{ scale: 0.98 }}
              className="w-full py-2.5 bg-amber-500 hover:bg-amber-400 disabled:opacity-50 disabled:cursor-not-allowed text-black font-semibold rounded-xl transition-colors flex items-center justify-center gap-2">
              {loading ? <div className="w-4 h-4 border-2 border-black/40 border-t-black rounded-full animate-spin" /> : (mode === 'login' ? <LogIn className="w-4 h-4" /> : <UserPlus className="w-4 h-4" />)}
              {loading ? 'Please wait...' : (mode === 'login' ? 'Sign In' : 'Create Account')}
            </motion.button>
          </form>

          <div className="mt-6 text-center">
            <button onClick={() => { setMode(mode === 'login' ? 'register' : 'login'); setError(''); }}
              className="text-sm text-zinc-500 hover:text-zinc-300 transition-colors">
              {mode === 'login' ? "Don't have an account? Register" : 'Already have an account? Sign in'}
            </button>
          </div>

          {mode === 'login' && (
            <div className="mt-4 p-3 rounded-xl bg-zinc-800/50 border border-white/5">
              <p className="text-xs text-zinc-500 text-center">Default admin: <span className="text-zinc-400">admin@artgallery.com</span> / <span className="text-zinc-400">Admin123!</span></p>
            </div>
          )}
        </div>
      </motion.div>
    </div>
  );
}
