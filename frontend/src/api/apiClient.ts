import axios from 'axios';

const backendPort = 5267;

export const apiClient = axios.create({
  baseURL: `http://127.0.0.1:${backendPort}/api`,
  timeout: 5000
});
