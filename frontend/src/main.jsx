import React from "react";
import App from "./App";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { AuthPage } from "./features/Auth/AuthPage";
import RequireAuth from "./guards/RequireAuth";
import RequireLocation from "./guards/RequireLocation";
import { LocationSetup } from "./features/LocationSetup/LocationSetup";
import { Settings } from "./features/Settings/Settings";
import "./main.scss";

import { MainPage } from "./features/MainPage/MainPage";
import { Favourites } from "./features/Favourites/Favourites";
import { Wardrobe } from "./features/Wardrobe/Wardrobe";

const router = createBrowserRouter([
  {
    path: "/auth",
    element: <AuthPage />,
  },
  {
    path: "/location-setup",
    element: (
      <RequireAuth>
        <LocationSetup />
      </RequireAuth>
    ),
  },
  {
    path: "/",
    element: (
      <RequireAuth>
        <RequireLocation>
          <App />
        </RequireLocation>
      </RequireAuth>
    ),
    children: [
      {
        index: true,
        element: <MainPage />,
      },
      {
        path: "main-page",
        element: <MainPage />,
      },
      {
        path: "favourites",
        element: <Favourites />,
      },
      {
        path: "wardrobe",
        element: <Wardrobe />,
      },
      {
        path: "settings",
        element: <Settings />,
      },
    ],
  },
]);

createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
);
