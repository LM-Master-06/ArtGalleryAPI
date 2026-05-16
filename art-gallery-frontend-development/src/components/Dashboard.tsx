import { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Image, Users, Tag, TrendingUp, Clock, DollarSign } from 'lucide-react';
import type { Artifact } from '../types';
import * as api from '../services/api';

interface StatCardProps {
  icon: React.ElementType;
  label: string;
  value: number | string;
  color: string;
  delay: number;
}

function StatCard({ icon: Icon, label, value, color, delay }: StatCardProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay }}
      className="relative group"
    >
      <div className="bg-zinc-900/50 border border-white/5 rounded-2xl p-6 hover:border-white/10 transition-colors">
        <div className="flex items-start justify-between mb-4">
          <div className={`w-10 h-10 rounded-xl ${color} flex items-center justify-center`}>
            <Icon className="w-5 h-5 text-white" />
          </div>
          <div className="flex items-center gap-1 text-emerald-400 text-xs font-medium">
            <TrendingUp className="w-3 h-3" />
            <span>Live</span>
          </div>
        </div>
        <div className="text-3xl font-bold text-white mb-1">{value}</div>
        <div className="text-sm text-zinc-500">{label}</div>
      </div>
    </motion.div>
  );
}

export default function Dashboard() {
  const [stats, setStats] = useState({ artifacts: 0, artists: 0, artTypes: 0 });
  const [recentArtifacts, setRecentArtifacts] = useState<Artifact[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [artifacts, artists, artTypes] = await Promise.all([
          api.getArtifacts(),
          api.getArtists(),
          api.getArtTypes(),
        ]);
        setStats({ artifacts: artifacts.length, artists: artists.length, artTypes: artTypes.length });
        setRecentArtifacts(artifacts.slice(0, 4));
      } catch (err) {
        console.error('Failed to fetch dashboard data:', err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5 }}>
        <h2 className="text-2xl font-bold text-white mb-2">Dashboard</h2>
        <p className="text-zinc-400">Overview of your Aboriginal art gallery</p>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <StatCard icon={Image} label="Total Artifacts" value={stats.artifacts} color="bg-amber-500/20" delay={0.1} />
        <StatCard icon={Users} label="Artists" value={stats.artists} color="bg-blue-500/20" delay={0.2} />
        <StatCard icon={Tag} label="Art Types" value={stats.artTypes} color="bg-purple-500/20" delay={0.3} />
      </div>

      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.3 }}
        className="bg-zinc-900/50 border border-white/5 rounded-2xl p-6"
      >
        <div className="flex items-center gap-2 mb-6">
          <Clock className="w-5 h-5 text-amber-400" />
          <h3 className="text-lg font-semibold text-white">Recently Added Artifacts</h3>
        </div>
        <div className="space-y-3">
          {recentArtifacts.length === 0 ? (
            <p className="text-zinc-500 text-sm">No artifacts yet. Add your first piece.</p>
          ) : (
            recentArtifacts.map((artifact, i) => (
              <motion.div
                key={artifact.id}
                initial={{ opacity: 0, x: -10 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: 0.4 + i * 0.1 }}
                className="flex items-center gap-4 p-3 rounded-xl bg-white/[0.02] hover:bg-white/[0.04] transition-colors"
              >
                <div className="w-12 h-12 rounded-lg bg-amber-500/10 border border-amber-500/20 flex items-center justify-center flex-shrink-0 overflow-hidden">
                  {artifact.imageUrl ? (
                    <img src={artifact.imageUrl} alt={artifact.title} className="w-full h-full object-cover" />
                  ) : (
                    <Image className="w-5 h-5 text-amber-400" />
                  )}
                </div>
                <div className="flex-1 min-w-0">
                  <div className="text-sm font-medium text-white truncate">{artifact.title}</div>
                  <div className="text-xs text-zinc-500">
                    {artifact.medium} &middot; {artifact.yearCreated}
                    {artifact.artist && <span> &middot; {artifact.artist.fullName}</span>}
                  </div>
                </div>
                {artifact.price != null && (
                  <div className="flex items-center gap-1 text-emerald-400 text-xs font-medium">
                    <DollarSign className="w-3 h-3" />
                    {artifact.price.toLocaleString()}
                  </div>
                )}
              </motion.div>
            ))
          )}
        </div>
      </motion.div>
    </div>
  );
}
