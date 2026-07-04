import { useState, useEffect, useCallback } from "react";
import { Link } from "react-router-dom";
import { problemsAPI } from "../services/api";
import { Navbar } from "./AIAssistant";

const DIFFICULTY_COLORS = {
  Easy: "bg-green-100 text-green-700",
  Medium: "bg-yellow-100 text-yellow-700",
  Hard: "bg-red-100 text-red-700",
};

export default function Problems() {
  const [problems, setProblems] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [difficulty, setDifficulty] = useState("");
  const [topic, setTopic] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ title: "", platform: "LeetCode", difficulty: "Medium", topic: "", notes: "", problemUrl: "" });
  const [formError, setFormError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchProblems = useCallback(async () => {
    setIsLoading(true);
    try {
      const response = await problemsAPI.getAll({ page, pageSize: 20, search: search || undefined, difficulty: difficulty || undefined, topic: topic || undefined });
      setProblems(response.data.items);
      setTotalCount(response.data.totalCount);
      setTotalPages(response.data.totalPages);
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  }, [page, search, difficulty, topic]);

  useEffect(() => { fetchProblems(); }, [fetchProblems]);
  useEffect(() => { setPage(1); }, [search, difficulty, topic]);
  useEffect(() => {
    const timer = setTimeout(() => setSearch(searchInput), 400);
    return () => clearTimeout(timer);
  }, [searchInput]);

  const handleAdd = async (e) => {
    e.preventDefault();
    setFormError("");
    setIsSubmitting(true);
    try {
      await problemsAPI.create(form);
      setShowForm(false);
      setForm({ title: "", platform: "LeetCode", difficulty: "Medium", topic: "", notes: "", problemUrl: "" });
      fetchProblems();
    } catch (err) {
      setFormError(err.response?.data?.message || "Failed to add problem.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this problem?")) return;
    try {
      await problemsAPI.delete(id);
      fetchProblems();
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="problems" />
      <div className="max-w-5xl mx-auto p-6">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-semibold text-gray-900">Problems</h1>
            <p className="text-gray-500 text-sm mt-0.5">{totalCount} problems solved</p>
          </div>
          <button onClick={() => setShowForm(!showForm)} className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700">+ Add Problem</button>
        </div>
        {showForm && (
          <div className="bg-white border border-gray-200 rounded-xl p-5 mb-6">
            <h2 className="font-medium text-gray-900 mb-4">Log a solved problem</h2>
            {formError && <p className="text-red-600 text-sm mb-3">{formError}</p>}
            <form onSubmit={handleAdd} className="grid grid-cols-2 gap-4">
              <input placeholder="Problem title *" value={form.title} onChange={e => setForm({...form, title: e.target.value})} required className="col-span-2 border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <select value={form.platform} onChange={e => setForm({...form, platform: e.target.value})} className="border border-gray-300 rounded-lg px-3 py-2 text-sm">
                {["LeetCode","HackerRank","CodeForces","GeeksForGeeks","Other"].map(p => <option key={p}>{p}</option>)}
              </select>
              <select value={form.difficulty} onChange={e => setForm({...form, difficulty: e.target.value})} className="border border-gray-300 rounded-lg px-3 py-2 text-sm">
                <option>Easy</option><option>Medium</option><option>Hard</option>
              </select>
              <input placeholder="Topic (e.g. Arrays, Trees) *" value={form.topic} onChange={e => setForm({...form, topic: e.target.value})} required className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <input placeholder="Problem URL (optional)" value={form.problemUrl} onChange={e => setForm({...form, problemUrl: e.target.value})} className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <textarea placeholder="Notes (optional)" value={form.notes} onChange={e => setForm({...form, notes: e.target.value})} rows={2} className="col-span-2 border border-gray-300 rounded-lg px-3 py-2 text-sm resize-none" />
              <div className="col-span-2 flex gap-2">
                <button type="submit" disabled={isSubmitting} className="px-4 py-2 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 disabled:opacity-50">{isSubmitting ? "Saving..." : "Save Problem"}</button>
                <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-gray-600 text-sm rounded-lg border border-gray-300 hover:bg-gray-50">Cancel</button>
              </div>
            </form>
          </div>
        )}
        <div className="flex gap-3 mb-4 flex-wrap">
          <input type="search" placeholder="Search problems..." value={searchInput} onChange={e => setSearchInput(e.target.value)} className="flex-1 min-w-[200px] border border-gray-300 rounded-lg px-3 py-2 text-sm" />
          <select value={difficulty} onChange={e => setDifficulty(e.target.value)} className="border border-gray-300 rounded-lg px-3 py-2 text-sm">
            <option value="">All difficulties</option>
            <option>Easy</option><option>Medium</option><option>Hard</option>
          </select>
          <select value={topic} onChange={e => setTopic(e.target.value)} className="border border-gray-300 rounded-lg px-3 py-2 text-sm">
            <option value="">All topics</option>
            {["Arrays","Strings","Trees","Graphs","Dynamic Programming","Recursion","Binary Search","Sorting","Hashing","Linked Lists","Stack","Queue"].map(t => <option key={t}>{t}</option>)}
          </select>
        </div>
        {isLoading ? (
          <div className="text-center py-12 text-gray-400">Loading...</div>
        ) : problems.length === 0 ? (
          <div className="text-center py-12 text-gray-400">No problems found. Add your first problem!</div>
        ) : (
          <div className="space-y-2">
            {problems.map(problem => (
              <div key={problem.problemId} className="bg-white border border-gray-200 rounded-xl p-4 flex items-center gap-4 hover:border-gray-300 transition-colors">
                <span className={`text-xs font-medium px-2.5 py-1 rounded-full flex-shrink-0 ${DIFFICULTY_COLORS[problem.difficulty]}`}>{problem.difficulty}</span>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <span className="font-medium text-gray-900 truncate">{problem.title}</span>
                    {problem.problemUrl && <a href={problem.problemUrl} target="_blank" rel="noopener noreferrer" className="text-blue-500 text-xs hover:underline flex-shrink-0">↗</a>}
                  </div>
                  <div className="flex items-center gap-2 mt-0.5 text-xs text-gray-500">
                    <span>{problem.topic}</span><span>·</span><span>{problem.platform}</span><span>·</span><span>{problem.solvedDate}</span>
                  </div>
                </div>
                <button onClick={() => handleDelete(problem.problemId)} className="text-gray-400 hover:text-red-500 text-sm flex-shrink-0">✕</button>
              </div>
            ))}
          </div>
        )}
        {totalPages > 1 && (
          <div className="flex items-center justify-between mt-6 text-sm text-gray-600">
            <span>Page {page} of {totalPages} ({totalCount} total)</span>
            <div className="flex gap-2">
              <button onClick={() => setPage(p => p - 1)} disabled={page === 1} className="px-3 py-1.5 border border-gray-300 rounded-lg disabled:opacity-40 hover:bg-gray-50">← Previous</button>
              <button onClick={() => setPage(p => p + 1)} disabled={page === totalPages} className="px-3 py-1.5 border border-gray-300 rounded-lg disabled:opacity-40 hover:bg-gray-50">Next →</button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
