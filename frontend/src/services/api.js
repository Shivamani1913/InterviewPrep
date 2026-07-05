import axios from "axios";

const API = axios.create({
  baseURL: "https://interviewprep-production-4d4f.up.railway.app/api",
  headers: { "Content-Type": "application/json" },
});

API.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

API.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export const authAPI = {
  register: (data) => API.post("/auth/register", data),
  login: (data) => API.post("/auth/login", data),
  getMe: () => API.get("/auth/me"),
  logout: () => API.post("/auth/logout"),
};

export const problemsAPI = {
  getAll: (params = {}) => API.get("/problems", { params }),
  getById: (id) => API.get(`/problems/${id}`),
  create: (data) => API.post("/problems", data),
  update: (id, data) => API.put(`/problems/${id}`, data),
  delete: (id) => API.delete(`/problems/${id}`),
};

export const notesAPI = {
  getAll: () => API.get("/notes"),
  getById: (id) => API.get(`/notes/${id}`),
  create: (data) => API.post("/notes", data),
  update: (id, data) => API.put(`/notes/${id}`, data),
  delete: (id) => API.delete(`/notes/${id}`),
};

export const contestsAPI = {
  getAll: () => API.get("/contests"),
  create: (data) => API.post("/contests", data),
  delete: (id) => API.delete(`/contests/${id}`),
};

export const goalsAPI = {
  getAll: () => API.get("/goals"),
  create: (data) => API.post("/goals", data),
  updateProgress: (id, currentCount) => API.put(`/goals/${id}/progress`, { currentCount }),
  delete: (id) => API.delete(`/goals/${id}`),
};

export const dashboardAPI = {
  get: () => API.get("/dashboard"),
};

export const aiAPI = {
  explain: (topic, difficulty) => API.post("/ai/explain", { topic, difficulty }),
  mockInterview: (topic, difficulty) => API.post("/ai/mock-interview", { topic, difficulty }),
  analyzeResume: (resumeText) => API.post("/ai/analyze-resume", { resumeText }),
};

export default API;
