import { useState, useEffect } from "react";
import { contestsAPI } from "../services/api";
import { Navbar } from "./AIAssistant";

export default function Contests() {
  const [contests, setContests] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState("");
  const [form, setForm] = useState({
    contestName: "", platform: "LeetCode", rank: "",
    ratingBefore: "", ratingAfter: "",
    contestDate: new Date().toISOString().split("T")[0],
  });

  const fetchContests = async () => {
    try {
      const res = await contestsAPI.getAll();
      setContests(res.data);
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => { fetchContests(); }, []);

  const handleAdd = async (e) => {
    e.preventDefault();
    setFormError("");
    setIsSubmitting(true);
    try {
      await contestsAPI.create({
        contestName: form.contestName, platform: form.platform,
        rank: parseInt(form.rank), ratingBefore: parseInt(form.ratingBefore),
        ratingAfter: parseInt(form.ratingAfter), contestDate: form.contestDate,
      });
      setShowForm(false);
      setForm({ contestName: "", platform: "LeetCode", rank: "", ratingBefore: "", ratingAfter: "", contestDate: new Date().toISOString().split("T")[0] });
      fetchContests();
    } catch (err) {
      setFormError(err.response?.data?.message || "Failed to add contest.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this contest?")) return;
    try {
      await contestsAPI.delete(id);
      fetchContests();
    } catch (err) {
      console.error(err);
    }
  };

  const totalRatingChange = contests.reduce((sum, c) => sum + c.ratingChange, 0);
  const bestRank = contests.length > 0 ? Math.min(...contests.map(c => c.rank)) : 0;

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="contests" />
      <div className="max-w-5xl mx-auto p-6">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-semibold text-gray-900">Contests</h1>
            <p className="text-gray-500 text-sm mt-0.5">{contests.length} contests tracked</p>
          </div>
          <button onClick={() => setShowForm(!showForm)} className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700">+ Add Contest</button>
        </div>
        <div className="grid grid-cols-3 gap-4 mb-6">
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <div className="text-3xl font-bold text-gray-900">{contests.length}</div>
            <div className="text-sm text-gray-500 mt-1">Total Contests</div>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <div className={`text-3xl font-bold ${totalRatingChange >= 0 ? "text-green-600" : "text-red-600"}`}>
              {totalRatingChange >= 0 ? "+" : ""}{totalRatingChange}
            </div>
            <div className="text-sm text-gray-500 mt-1">Total Rating Change</div>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <div className="text-3xl font-bold text-blue-600">{bestRank || "-"}</div>
            <div className="text-sm text-gray-500 mt-1">Best Rank</div>
          </div>
        </div>
        {showForm && (
          <div className="bg-white border border-gray-200 rounded-xl p-5 mb-6">
            <h2 className="font-medium text-gray-900 mb-4">Add Contest</h2>
            {formError && <p className="text-red-600 text-sm mb-3">{formError}</p>}
            <form onSubmit={handleAdd} className="grid grid-cols-2 gap-4">
              <input placeholder="Contest name *" value={form.contestName} onChange={e => setForm({...form, contestName: e.target.value})} required className="col-span-2 border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <select value={form.platform} onChange={e => setForm({...form, platform: e.target.value})} className="border border-gray-300 rounded-lg px-3 py-2 text-sm">
                {["LeetCode","CodeForces","HackerRank","CodeChef","Other"].map(p => <option key={p}>{p}</option>)}
              </select>
              <input type="number" placeholder="Your rank *" value={form.rank} onChange={e => setForm({...form, rank: e.target.value})} required className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <input type="number" placeholder="Rating before *" value={form.ratingBefore} onChange={e => setForm({...form, ratingBefore: e.target.value})} required className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <input type="number" placeholder="Rating after *" value={form.ratingAfter} onChange={e => setForm({...form, ratingAfter: e.target.value})} required className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <input type="date" value={form.contestDate} onChange={e => setForm({...form, contestDate: e.target.value})} required className="border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <div className="col-span-2 flex gap-2">
                <button type="submit" disabled={isSubmitting} className="px-4 py-2 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 disabled:opacity-50">{isSubmitting ? "Saving..." : "Save Contest"}</button>
                <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-gray-600 text-sm rounded-lg border border-gray-300 hover:bg-gray-50">Cancel</button>
              </div>
            </form>
          </div>
        )}
        {isLoading ? (
          <div className="text-center py-12 text-gray-400">Loading...</div>
        ) : contests.length === 0 ? (
          <div className="text-center py-12 text-gray-400">No contests yet. Add your first contest!</div>
        ) : (
          <div className="space-y-2">
            {contests.map(contest => (
              <div key={contest.contestId} className="bg-white border border-gray-200 rounded-xl p-4 flex items-center gap-4 hover:border-gray-300 transition-colors">
                <div className="flex-1">
                  <div className="flex items-center gap-3">
                    <span className="font-medium text-gray-900">{contest.contestName}</span>
                    <span className="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded-full">{contest.platform}</span>
                  </div>
                  <div className="flex items-center gap-4 mt-1 text-xs text-gray-500">
                    <span>Rank: <span className="font-medium text-gray-700">#{contest.rank}</span></span>
                    <span>Rating: {contest.ratingBefore} → {contest.ratingAfter}</span>
                    <span>{contest.contestDate}</span>
                  </div>
                </div>
                <div className={`text-lg font-bold flex-shrink-0 ${contest.ratingChange >= 0 ? "text-green-600" : "text-red-600"}`}>
                  {contest.ratingChange >= 0 ? "+" : ""}{contest.ratingChange}
                </div>
                <button onClick={() => handleDelete(contest.contestId)} className="text-gray-400 hover:text-red-500 text-sm flex-shrink-0">✕</button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
