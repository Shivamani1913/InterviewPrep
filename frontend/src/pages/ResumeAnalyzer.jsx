import { useState } from "react";
import { aiAPI } from "../services/api";
import { Navbar } from "./AIAssistant";

export default function ResumeAnalyzer() {
  const [resumeText, setResumeText] = useState("");
  const [result, setResult] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const handleAnalyze = async () => {
    if (!resumeText.trim()) {
      setError("Please paste your resume text.");
      return;
    }
    if (resumeText.trim().length < 100) {
      setError("Resume text is too short. Please paste more content.");
      return;
    }
    setError("");
    setResult("");
    setIsLoading(true);
    try {
      const res = await aiAPI.analyzeResume(resumeText);
      setResult(res.data.result);
    } catch (err) {
      setError("Failed to analyze resume. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="resume" />
      <div className="max-w-5xl mx-auto p-6">
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">Resume Analyzer</h1>
          <p className="text-gray-500 text-sm mt-1">Get AI-powered feedback on your resume</p>
        </div>

        <div className="grid grid-cols-2 gap-6">
          <div>
            <div className="bg-white border border-gray-200 rounded-xl p-5">
              <h2 className="font-medium text-gray-900 mb-3">Paste Your Resume</h2>
              <p className="text-xs text-gray-400 mb-3">
                Copy all text from your resume and paste it below
              </p>
              <textarea
                value={resumeText}
                onChange={e => setResumeText(e.target.value)}
                placeholder="Paste your resume text here...

Example:
John Doe
Software Engineer
john@email.com

SKILLS
- Python, JavaScript, React
- Node.js, .NET Core
- SQL Server, MongoDB

EXPERIENCE
Software Developer at XYZ Company (2022-Present)
- Built REST APIs using .NET
- Developed React frontend...

EDUCATION
B.Tech Computer Science (2022)"
                rows={20}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm resize-none font-mono"
              />
              <div className="flex items-center justify-between mt-3">
                <span className="text-xs text-gray-400">
                  {resumeText.length} characters
                </span>
                <button
                  onClick={() => setResumeText("")}
                  className="text-xs text-gray-400 hover:text-gray-600"
                >
                  Clear
                </button>
              </div>
              {error && <p className="text-red-600 text-sm mt-2">{error}</p>}
              <button
                onClick={handleAnalyze}
                disabled={isLoading || !resumeText.trim()}
                className="w-full mt-4 py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition-colors"
              >
                {isLoading ? "Analyzing..." : "Analyze Resume"}
              </button>
            </div>
          </div>

          <div>
            {isLoading && (
              <div className="bg-white border border-gray-200 rounded-xl p-6 flex flex-col items-center justify-center h-64 gap-4">
                <div className="w-8 h-8 border-2 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
                <div className="text-center">
                  <p className="text-gray-700 font-medium">Analyzing your resume...</p>
                  <p className="text-gray-400 text-sm mt-1">This may take a few seconds</p>
                </div>
              </div>
            )}

            {!isLoading && !result && (
              <div className="bg-white border border-gray-200 rounded-xl p-6 flex flex-col items-center justify-center h-64 text-center">
                <div className="text-4xl mb-3">📄</div>
                <p className="text-gray-500 text-sm">Paste your resume on the left and click Analyze to get AI feedback</p>
                <div className="mt-4 text-left space-y-2">
                  <p className="text-xs text-gray-400 font-medium">You will get:</p>
                  <p className="text-xs text-gray-400">✓ Detected skills</p>
                  <p className="text-xs text-gray-400">✓ Missing skills</p>
                  <p className="text-xs text-gray-400">✓ Improvement suggestions</p>
                  <p className="text-xs text-gray-400">✓ Overall rating out of 10</p>
                </div>
              </div>
            )}

            {result && (
              <div className="bg-white border border-gray-200 rounded-xl p-5">
                <div className="flex items-center gap-2 mb-4">
                  <span className="text-lg">🤖</span>
                  <h2 className="font-medium text-gray-900">AI Analysis</h2>
                  <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full ml-auto">
                    Powered by Gemini
                  </span>
                </div>
                <div className="overflow-y-auto max-h-[600px]">
                  <pre className="whitespace-pre-wrap text-sm text-gray-700 font-sans leading-relaxed">
                    {result}
                  </pre>
                </div>
                <button
                  onClick={() => { setResult(""); setResumeText(""); }}
                  className="mt-4 w-full py-2 border border-gray-300 text-gray-600 rounded-lg text-sm hover:bg-gray-50"
                >
                  Analyze Another Resume
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
