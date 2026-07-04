import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { dashboardAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";
import { PieChart, Pie, Cell, LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";

const COLORS = ["#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#ec4899"];

const Navbar = ({ activePage }) => {
  const { user, logout } = useAuth();
  return (
    <nav className="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
      <div className="flex items-center gap-8">
        <span className="font-semibold text-gray-900">Interview Prep</span>
        <div className="flex gap-4 text-sm">
          <Link to="/dashboard" className={activePage === "dashboard" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Dashboard</Link>
          <Link to="/problems" className={activePage === "problems" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Problems</Link>
          <Link to="/notes" className={activePage === "notes" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Notes</Link>
          <Link to="/contests" className={activePage === "contests" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Contests</Link>
          <Link to="/goals" className={activePage === "goals" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Goals</Link>
        </div>
      </div>
      <div className="flex items-center gap-4">
        <span className="text-sm text-gray-600">Hi, {user?.name}</span>
        <button onClick={logout} className="text-sm text-red-500 hover:text-red-700">Logout</button>
      </div>
    </nav>
  );
};

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    dashboardAPI.get()
      .then(res => setData(res.data))
      .catch(err => console.error(err))
      .finally(() => setIsLoading(false));
  }, []);

  if (isLoading) return <div className="min-h-screen flex items-center justify-center text-gray-500">Loading dashboard...</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="dashboard" />
      <div className="max-w-6xl mx-auto p-6">
        <h1 className="text-2xl font-semibold text-gray-900 mb-6">Dashboard</h1>
        <div className="grid grid-cols-4 gap-4 mb-6">
          {[
            { label: "Total Solved", value: data?.totalSolved, color: "text-gray-900" },
            { label: "Easy", value: data?.easySolved, color: "text-green-600" },
            { label: "Medium", value: data?.mediumSolved, color: "text-yellow-600" },
            { label: "Hard", value: data?.hardSolved, color: "text-red-600" },
          ].map(card => (
            <div key={card.label} className="bg-white rounded-xl border border-gray-200 p-5">
              <div className={`text-3xl font-bold ${card.color}`}>{card.value}</div>
              <div className="text-sm text-gray-500 mt-1">{card.label}</div>
            </div>
          ))}
        </div>
        <div className="grid grid-cols-2 gap-4 mb-6">
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <h2 className="font-medium text-gray-900 mb-4">Topic Breakdown</h2>
            {data?.topicBreakdown?.length > 0 ? (
              <ResponsiveContainer width="100%" height={200}>
                <PieChart>
                  <Pie data={data.topicBreakdown} dataKey="count" nameKey="topic" cx="50%" cy="50%" outerRadius={80} label={({ topic, count }) => `${topic}: ${count}`}>
                    {data.topicBreakdown.map((_, i) => <Cell key={i} fill={COLORS[i % COLORS.length]} />)}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <div className="h-48 flex items-center justify-center text-gray-400 text-sm">Add problems to see chart</div>
            )}
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <h2 className="font-medium text-gray-900 mb-4">Monthly Progress</h2>
            {data?.monthlyProgress?.length > 0 ? (
              <ResponsiveContainer width="100%" height={200}>
                <LineChart data={data.monthlyProgress}>
                  <XAxis dataKey="month" tick={{ fontSize: 11 }} />
                  <YAxis />
                  <Tooltip />
                  <Line type="monotone" dataKey="count" stroke="#3b82f6" strokeWidth={2} dot={{ r: 4 }} />
                </LineChart>
              </ResponsiveContainer>
            ) : (
              <div className="h-48 flex items-center justify-center text-gray-400 text-sm">Add problems to see chart</div>
            )}
          </div>
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <h2 className="font-medium text-gray-900 mb-4">Streak</h2>
            <div className="flex items-center gap-6">
              <div className="text-center">
                <div className="text-4xl font-bold text-orange-500">{data?.streak?.currentStreak}</div>
                <div className="text-sm text-gray-500 mt-1">Current Streak</div>
              </div>
              <div className="text-center">
                <div className="text-4xl font-bold text-gray-700">{data?.streak?.longestStreak}</div>
                <div className="text-sm text-gray-500 mt-1">Longest Streak</div>
              </div>
              <div className="text-center">
                <div className={`text-2xl font-bold ${data?.streak?.solvedToday ? "text-green-500" : "text-gray-400"}`}>
                  {data?.streak?.solvedToday ? "✓" : "✗"}
                </div>
                <div className="text-sm text-gray-500 mt-1">Solved Today</div>
              </div>
            </div>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <h2 className="font-medium text-gray-900 mb-4">Active Goals</h2>
            {data?.activeGoals?.length > 0 ? (
              <div className="space-y-3">
                {data.activeGoals.map(goal => (
                  <div key={goal.goalId}>
                    <div className="flex justify-between text-sm mb-1">
                      <span className="text-gray-700">{goal.description}</span>
                      <span className="text-gray-500">{goal.currentCount}/{goal.targetCount}</span>
                    </div>
                    <div className="w-full bg-gray-100 rounded-full h-2">
                      <div className="bg-blue-500 h-2 rounded-full" style={{ width: `${Math.min(goal.progressPercentage, 100)}%` }} />
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-gray-400 text-sm">No active goals yet.</div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
