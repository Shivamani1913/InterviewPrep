import { useState, useEffect } from "react";
import { notesAPI } from "../services/api";
import { Navbar } from "./AIAssistant";

export default function Notes() {
  const [notes, setNotes] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ title: "", topic: "", content: "" });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [selected, setSelected] = useState(null);

  const fetchNotes = async () => {
    try {
      const res = await notesAPI.getAll();
      setNotes(res.data);
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => { fetchNotes(); }, []);

  const handleAdd = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await notesAPI.create(form);
      setShowForm(false);
      setForm({ title: "", topic: "", content: "" });
      fetchNotes();
    } catch (err) {
      console.error(err);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this note?")) return;
    try {
      await notesAPI.delete(id);
      if (selected?.noteId === id) setSelected(null);
      fetchNotes();
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar activePage="notes" />
      <div className="max-w-6xl mx-auto p-6">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">Notes</h1>
          <button onClick={() => setShowForm(!showForm)} className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700">+ Add Note</button>
        </div>
        {showForm && (
          <div className="bg-white border border-gray-200 rounded-xl p-5 mb-6">
            <h2 className="font-medium text-gray-900 mb-4">New Note</h2>
            <form onSubmit={handleAdd} className="space-y-3">
              <input placeholder="Title *" value={form.title} onChange={e => setForm({...form, title: e.target.value})} required className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <input placeholder="Topic (e.g. Trees, DP) *" value={form.topic} onChange={e => setForm({...form, topic: e.target.value})} required className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm" />
              <textarea placeholder="Write your notes here..." value={form.content} onChange={e => setForm({...form, content: e.target.value})} required rows={6} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm resize-none" />
              <div className="flex gap-2">
                <button type="submit" disabled={isSubmitting} className="px-4 py-2 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 disabled:opacity-50">{isSubmitting ? "Saving..." : "Save Note"}</button>
                <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-gray-600 text-sm rounded-lg border border-gray-300 hover:bg-gray-50">Cancel</button>
              </div>
            </form>
          </div>
        )}
        <div className="grid grid-cols-3 gap-4">
          <div className="col-span-1 space-y-2">
            {isLoading ? (
              <div className="text-center py-8 text-gray-400">Loading...</div>
            ) : notes.length === 0 ? (
              <div className="text-center py-8 text-gray-400 text-sm">No notes yet.</div>
            ) : notes.map(note => (
              <div key={note.noteId} onClick={() => setSelected(note)} className={`bg-white border rounded-xl p-4 cursor-pointer hover:border-blue-300 transition-colors ${selected?.noteId === note.noteId ? "border-blue-400" : "border-gray-200"}`}>
                <div className="font-medium text-gray-900 text-sm truncate">{note.title}</div>
                <div className="text-xs text-blue-600 mt-1">{note.topic}</div>
                <div className="text-xs text-gray-400 mt-1">{new Date(note.createdDate).toLocaleDateString()}</div>
              </div>
            ))}
          </div>
          <div className="col-span-2">
            {selected ? (
              <div className="bg-white border border-gray-200 rounded-xl p-6">
                <div className="flex items-start justify-between mb-4">
                  <div>
                    <h2 className="text-xl font-semibold text-gray-900">{selected.title}</h2>
                    <span className="text-sm text-blue-600">{selected.topic}</span>
                  </div>
                  <button onClick={() => handleDelete(selected.noteId)} className="text-gray-400 hover:text-red-500 text-sm">Delete</button>
                </div>
                <pre className="text-sm text-gray-700 whitespace-pre-wrap font-sans leading-relaxed">{selected.content}</pre>
              </div>
            ) : (
              <div className="bg-white border border-gray-200 rounded-xl p-6 flex items-center justify-center h-64 text-gray-400 text-sm">Select a note to view its content</div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
