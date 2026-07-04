import { useState } from "react";
import { Link } from "react-router-dom";
import { aiAPI } from "../services/api";
import { useAuth } from "../context/AuthContext";

const TOPICS = [
  "Arrays", "Strings", "Linked Lists", "Trees", "Graphs",
  "Dynamic Programming", "Recursion", "Binary Search",
  "Sorting", "Hashing", "Stack", "Queue", "Heap",
  "Kadane Algorithm", "Sliding Window", "Two Pointers",
  "Greedy", "Backtracking", "Trie", "Segment Tree"
];

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
          <Link to="/ai" className={activePage === "ai" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>AI Assistant</Link>
          <Link to="/mock-interview" className={activePage === "mock" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Mock Interview</Link>
          <Link to="/resume" className={activePage === "resume" ? "text-blue-600 font-medium" : "text-gray-600 hover:text-gray-900"}>Resume</Link>
        </div>
      </div>
      <div className="flex items-center gap-4">
        <span className="text-sm text-gray-600">Hi, {user?.name}</span>
        <button onClick={logout} className="text-sm text-red-500 hover:text-red-700">Logout</button>
      </div>
    </nav>
  );
};

export { Navbar };

export default function AIAssistant() {
  const [topic, setTopic] = useState("");
  const [customTopic, setCustomTopic] = useState("");
  const [difficulty, setDifficulty] = useState("Medium");
  const [result, setResult] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const handleExplain = async () => {
    const selectedTopic = customTopic || topic;
    if (!selectedTopic) {
      setError("Please select or enter a topic.");
      return;
    }
    setError("");
    setResult("");
    setIsLoading(true);
    try {
      const res = await aiAPI.explain(selectedTopic, difficulty);
      setResult(res.data.result);
    } catch (err) {
      setError("Failed to get explanation. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="ai" />
      <div className="max-w-4xl mx-auto p-6">
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">AI Assistant</h1>
          <p className="text-gray-500 text-sm mt-1">Get instant explanations for any DSA topic</p>
        </div>

        <div className="bg-white border border-gray-200 rounded-xl p-6 mb-6">
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Select Topic</label>
              <select
                value={topic}
                onChange={e => { setTopic(e.target.value); setCustomTopic(""); }}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
              >
                <option value="">-- Select a topic --</option>
                {TOPICS.map(t => <option key={t}>{t}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Or Type Custom Topic</label>
              <input
                type="text"
                placeholder="e.g. Kadane's Algorithm"
                value={customTopic}
                onChange={e => { setCustomTopic(e.target.value); setTopic(""); }}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
              />
            </div>
          </div>

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-1">Difficulty Level</label>
            <div className="flex gap-3">
              {["Easy", "Medium", "Hard"].map(d => (
                <button
                  key={d}
                  onClick={() => setDifficulty(d)}
                  className={`px-4 py-1.5 rounded-lg text-sm font-medium border transition-colors ${
                    difficulty === d
                      ? d === "Easy" ? "bg-green-100 text-green-700 border-green-300"
                        : d === "Medium" ? "bg-yellow-100 text-yellow-700 border-yellow-300"
                        : "bg-red-100 text-red-700 border-red-300"
                      : "bg-white text-gray-600 border-gray-300 hover:bg-gray-50"
                  }`}
                >
                  {d}
                </button>
              ))}
            </div>
          </div>

          {error && <p className="text-red-600 text-sm mb-3">{error}</p>}

          <button
            onClick={handleExplain}
            disabled={isLoading}
            className="w-full py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition-colors"
          >
            {isLoading ? "AI is thinking..." : "Explain Topic"}
          </button>
        </div>

        {isLoading && (
          <div className="bg-white border border-gray-200 rounded-xl p-6 flex items-center gap-3">
            <div className="w-5 h-5 border-2 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
            <span className="text-gray-500 text-sm">Generating explanation...</span>
          </div>
        )}

        {result && (
          <div className="bg-white border border-gray-200 rounded-xl p-6">
            <div className="flex items-center gap-2 mb-4">
              <span className="text-blue-600 text-lg">🤖</span>
              <h2 className="font-medium text-gray-900">AI Explanation</h2>
              <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full ml-auto">
                Powered by Gemini
              </span>
            </div>
            <div className="prose prose-sm max-w-none">
              <pre className="whitespace-pre-wrap text-sm text-gray-700 font-sans leading-relaxed">
                {result}
              </pre>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
