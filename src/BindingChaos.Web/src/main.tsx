import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { API_CONFIG } from './config/api'

// Mint a CSRF token for anonymous visitors on first load
fetch(`${API_CONFIG.baseUrl}/api/v1/auth/csrf`, {
  method: 'GET',
  credentials: 'include',
}).catch(() => {/* ignore */});

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
