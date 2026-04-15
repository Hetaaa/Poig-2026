import { useEffect, useState } from 'react';
import { apiClient } from './api/apiClient';

type WeatherStyle = {
  id: string;
  name: string;
  themeColor: string;
};

export default function App() {
  const [styles, setStyles] = useState<WeatherStyle[]>([]);

  useEffect(() => {
    apiClient
      .get<WeatherStyle[]>('/WeatherStyles')
      .then((response) => setStyles(response.data))
      .catch(() => setStyles([]));
  }, []);

  return (
    <main style={{ fontFamily: 'sans-serif', padding: 16 }}>
      <h1>WeatherStyler</h1>
      <p>Połączenie z backend sidecar przez HTTP.</p>
      <ul>
        {styles.map((style) => (
          <li key={style.id}>
            {style.name} ({style.themeColor})
          </li>
        ))}
      </ul>
    </main>
  );
}
