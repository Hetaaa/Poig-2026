#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use std::sync::Mutex;
use tauri::{Manager, RunEvent};
use tauri_plugin_shell::process::CommandChild;
use tauri_plugin_shell::ShellExt;

// Struktura przechowująca referencję do działającego procesu sidecara .NET
struct BackendState(Mutex<Option<CommandChild>>);

fn main() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .manage(BackendState(Mutex::new(None)))
       .setup(|app| {
            // Pobieramy absolutną ścieżkę do folderu z aplikacją, gdzie leżą binaries
            let resource_dir = app.path().resource_dir()?;
            let binaries_dir = resource_dir.join("binaries");

            // Tworzymy konfigurację sidecara
            let mut sidecar = app
                .shell()
                .sidecar("backend")?;

            // !!! HACK SYSTEMOWY !!!
            // Wstrzykujemy folder binaries bezpośrednio do PATH procesu .NET.
            // Dzięki temu Windows ZAWSZE znajdzie e_sqlite3.dll obok pliku .exe!
            if cfg!(target_os = "windows") {
                sidecar = sidecar.env("PATH", format!("{};{}", binaries_dir.display(), std::env::var("PATH").unwrap_or_default()));
            }

            let (mut rx, child) = sidecar.spawn()?;

            // WĄTEK LOGÓW (Zostaje bez zmian)
            tauri::async_runtime::spawn(async move {
                use tauri_plugin_shell::process::CommandEvent;
                while let Some(event) = rx.recv().await {
                    match event {
                        CommandEvent::Stdout(line) => {
                            if let Ok(text) = String::from_utf8(line) {
                                println!("[.NET STDOUT] {}", text.trim());
                            }
                        }
                        CommandEvent::Stderr(line) => {
                            if let Ok(text) = String::from_utf8(line) {
                                eprintln!("[.NET STDERR] {}", text.trim());
                            }
                        }
                        _ => {}
                    }
                }
            });

            let state = app.state::<BackendState>();
            let mut guard = state.0.lock().expect("backend state lock");
            *guard = Some(child);

            Ok(())
        })
        .build(tauri::generate_context!())
        .expect("Błąd podczas budowania aplikacji Tauri")
        .run(|app_handle, event| {
            // Bezpieczne i czyste zamykanie procesu bocznego przy wyjściu z aplikacji
            if let RunEvent::Exit = event {
                if let Some(backend_state) = app_handle.try_state::<BackendState>() {
                    if let Ok(mut guard) = backend_state.0.lock() {
                        // .take() wyciąga proces z Option (zostawiając None) i daje nam do niego pełną własność
                        if let Some(child) = guard.take() {
                            let _ = child.kill();
                        }
                    }
                }
            }
        });
}