import { useState, useEffect } from "react";
import { goalsAPI } from "../services/api";
import { Navbar } from "./AIAssistant";

export default function Goals() {
  const [goals, setGoals] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingProgress, setEditingProgress] = useState(null);
  const [newProgress, setNewProgress] = useState("");
  const [form, setForm] = useState({ description: "", targetCount: "", deadline: "" });

  const fetchGoals = async () => {
    try {
      const res = await goalsAPI.getAll();
      setGoals(res.data);
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => { fetchGoals(); }, []);

  const handleAdd = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await goalsAPI.create({ description: form.description, targetCount: parseInt(form.targetCount), deadline: form.deadline || null });
      setShowForm(false);
      setForm({ description: "", targetCount: "", deadline: "" });
      fetchGoals();
    } catch (err) {
      console.error(err);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleUpdateProgress = async (goalId) => {
    try {
      await goalsAPI.updateProgress(goalId, parseInt(newProgress));
      setEditingProgress(null);
      setNewProgress("");
      fetchGoals();
    } catch (err) {
      console.error(err);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this goal?")) return;
    try {
      await goalsAPI.delete(id);
      fetchGoals();
    } catch (err) {
      console.error(err);
    }
  };

  const activeGoals = goals.filter(g => !g.isCompleted);
  const completedGoals = goals.filter(g => g.isCompleted);

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="goals" />
      <div className="max-w-4xl mx-auto p-6">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-semibold text-gray-900">Goals</h1>
            <p className="text-gray-500 text-sm mt-0.5">{activeGoals.length} active · {completedGoals.length} completed</p>
          </div>
          <button onClick={() => setShowForm(!showForm)} className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700">+ Add Goal</button>
        </div>
        {showForm && (
          <div className="bg-white border border-gray-200 rounded-xl p-5 mb-6">
            <h2 className="font-medium text-gray-900 mb-4">New Goal</h2>
            <form onSubmit={handleAdd} className="space-y-3">
              <input placeholder="Goal description * (e.g. Solve 300 problems)" value={form.description} onChange={e => setForm({...form, description: e.target.value})} required className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <div className="grid grid-cols-2 gap-3">
                <input type="number" placeholder="Target count * (e.g. 300)" value={form.targetCount} onChange={e => setForm({...form, targetCount: e.target.value})} required className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
                <input type="date" value={form.deadline} onChange={e => setForm({...form, deadline: e.target.value})} className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              </div>
              <div className="flex gap-2">
                <button type="submit" disabled={isSubmitting} className="px-4 py-2 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 disabled:opacity-50">{isSubmitting ? "Saving..." : "Save Goal"}</button>
                <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-gray-600 text-sm rounded-lg border border-gray-300 hover:bg-gray-50">Cancel</button>
              </div>
            </form>
          </div>
        )}
        {isLoading ? (
          <div className="text-center py-12 text-gray-400">Loading...</div>
        ) : goals.length === 0 ? (
          <div className="text-center py-12 text-gray-400">No goals yet. Set your first goal!</div>
        ) : (
          <div className="space-y-4">
            {activeGoals.length > 0 && (
              <div>
                <h2 className="text-sm font-medium text-gray-500 uppercase tracking-wide mb-3">Active Goals</h2>
                <div className="space-y-3">
                  {activeGoals.map(goal => (
                    <div key={goal.goalId} className="bg-white border border-gray-200 rounded-xl p-5">
                      <div className="flex items-start justify-between mb-3">
                        <div>
                          <div className="font-medium text-gray-900">{goal.description}</div>
                          {goal.deadline && <div className="text-xs text-gray-400 mt-0.5">Deadline: {new Date(goal.deadline).toLocaleDateString()}</div>}
                        </div>
                        <button onClick={() => handleDelete(goal.goalId)} className="text-gray-400 hover:text-red-500 text-sm">✕</button>
                      </div>
                      <div className="flex items-center gap-3 mb-2">
                        <div className="flex-1 bg-gray-100 rounded-full h-3">
                          <div className="bg-blue-500 h-3 rounded-full transition-all" style={{ width: `${Math.min(goal.progressPercentage, 100)}%` }} />
                        </div>
                        <span className="text-sm font-medium text-gray-700 flex-shrink-0">{goal.progressPercentage}%</span>
                      </div>
                      <div className="flex items-center justify-between">
                        <span className="text-sm text-gray-500">{goal.currentCount} / {goal.targetCount}</span>
                        {editingProgress === goal.goalId ? (
                          <div className="flex items-center gap-2">
                            <input type="number" value={newProgress} onChange={e => setNewProgress(e.target.value)} placeholder="New count" className="border border-gray-300 rounded px-2 py-1 text-sm w-24" />
                            <button onClick={() => handleUpdateProgress(goal.goalId)} className="px-3 py-1 bg-blue-600 text-white text-xs rounded-lg hover:bg-blue-700">Save</button>
                            <button onClick={() => setEditingProgress(null)} className="px-3 py-1 border border-gray-300 text-gray-600 text-xs rounded-lg hover:bg-gray-50">Cancel</button>
                          </div>
                        ) : (
                          <button onClick={() => { setEditingProgress(goal.goalId); setNewProgress(goal.currentCount.toString()); }} className="text-xs text-blue-600 hover:underline">Update progress</button>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
            {completedGoals.length > 0 && (
              <div>
                <h2 className="text-sm font-medium text-gray-500 uppercase tracking-wide mb-3">Completed Goals</h2>
                <div className="space-y-3">
                  {completedGoals.map(goal => (
                    <div key={goal.goalId} className="bg-white border border-gray-200 rounded-xl p-5 opacity-70">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                          <span className="text-green-500 text-lg">✓</span>
                          <div>
                            <div className="font-medium text-gray-900">{goal.description}</div>
                            <div className="text-xs text-gray-400">{goal.targetCount} / {goal.targetCount} completed</div>
                          </div>
                        </div>
                        <button onClick={() => handleDelete(goal.goalId)} className="text-gray-400 hover:text-red-500 text-sm">✕</button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
