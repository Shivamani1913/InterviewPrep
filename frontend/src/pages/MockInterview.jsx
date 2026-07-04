import { useState } from "react";
import { Link } from "react-router-dom";
import { aiAPI } from "../services/api";
import { Navbar } from "./AIAssistant";

const TOPICS = [
  "Arrays", "Strings", "Linked Lists", "Trees", "Graphs",
  "Dynamic Programming", "Recursion", "Binary Search",
  "Sorting", "Hashing", "Stack", "Queue", "Heap",
  "Sliding Window", "Two Pointers", "Greedy", "Backtracking"
];

export default function MockInterview() {
  const [topic, setTopic] = useState("Arrays");
  const [difficulty, setDifficulty] = useState("Medium");
  const [result, setResult] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [showHints, setShowHints] = useState(false);

  const handleGenerate = async () => {
    setError("");
    setResult("");
    setShowHints(false);
    setIsLoading(true);
    try {
      const res = await aiAPI.mockInterview(topic, difficulty);
      setResult(res.data.result);
    } catch (err) {
      setError("Failed to generate interview question. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="mock" />
      <div className="max-w-4xl mx-auto p-6">
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">Mock Interview</h1>
          <p className="text-gray-500 text-sm mt-1">Practice with AI-generated interview questions</p>
        </div>

        <div className="bg-white border border-gray-200 rounded-xl p-6 mb-6">
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Topic</label>
              <select
                value={topic}
                onChange={e => setTopic(e.target.value)}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
              >
                {TOPICS.map(t => <option key={t}>{t}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Difficulty</label>
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
          </div>

          {error && <p className="text-red-600 text-sm mb-3">{error}</p>}

          <button
            onClick={handleGenerate}
            disabled={isLoading}
            className="w-full py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition-colors"
          >
            {isLoading ? "Generating question..." : "Generate Interview Question"}
          </button>
        </div>

        {isLoading && (
          <div className="bg-white border border-gray-200 rounded-xl p-6 flex items-center gap-3">
            <div className="w-5 h-5 border-2 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
            <span className="text-gray-500 text-sm">Generating your interview question...</span>
          </div>
        )}

        {result && (
          <div className="bg-white border border-gray-200 rounded-xl p-6">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-2">
                <span className="text-lg">🎯</span>
                <h2 className="font-medium text-gray-900">Interview Question</h2>
                <span className={`text-xs px-2 py-0.5 rounded-full ${
                  difficulty === "Easy" ? "bg-green-100 text-green-700"
                  : difficulty === "Medium" ? "bg-yellow-100 text-yellow-700"
                  : "bg-red-100 text-red-700"
                }`}>{difficulty}</span>
              </div>
              <button
                onClick={handleGenerate}
                className="text-sm text-blue-600 hover:underline"
              >
                Generate Another
              </button>
            </div>
            <pre className="whitespace-pre-wrap text-sm text-gray-700 font-sans leading-relaxed">
              {result}
            </pre>
            <div className="mt-4 pt-4 border-t border-gray-100">
              <div className="flex gap-3">
                <button
                  onClick={() => setShowHints(!showHints)}
                  className="px-4 py-2 bg-yellow-50 text-yellow-700 border border-yellow-200 rounded-lg text-sm hover:bg-yellow-100"
                >
                  {showHints ? "Hide Timer" : "Start Timer ⏱"}
                </button>
                {showHints && <Timer />}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function Timer() {
  const [seconds, setSeconds] = useState(0);
  const [running, setRunning] = useState(true);

  useState(() => {
    if (!running) return;
    const interval = setInterval(() => setSeconds(s => s + 1), 1000);
    return () => clearInterval(interval);
  }, [running]);

  const mins = Math.floor(seconds / 60).toString().padStart(2, "0");
  const secs = (seconds % 60).toString().padStart(2, "0");

  return (
    <div className="flex items-center gap-3">
      <span className="text-2xl font-mono font-bold text-gray-800">{mins}:{secs}</span>
      <button onClick={() => setRunning(!running)} className="text-sm text-gray-500 hover:text-gray-700">
        {running ? "Pause" : "Resume"}
      </button>
      <button onClick={() => setSeconds(0)} className="text-sm text-gray-500 hover:text-gray-700">
        Reset
      </button>
    </div>
  );
}
