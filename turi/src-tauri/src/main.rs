use std::sync::Mutex;

use tauri::{Manager, RunEvent, State};
use tauri_plugin_shell::process::CommandChild;
use tauri_plugin_shell::ShellExt;

struct BackendState(Mutex<Option<CommandChild>>);

fn main() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .manage(BackendState(Mutex::new(None)))
        .setup(|app| {
            let sidecar = app
                .shell()
                .sidecar("backend")?
                .args(["--urls", "http://127.0.0.1:5267"]);

            let (_rx, child) = sidecar.spawn()?;

            let state = app.state::<BackendState>();
            let mut guard = state.0.lock().expect("backend state lock");
            *guard = Some(child);

            Ok(())
        })
        .build(tauri::generate_context!())
        .expect("error while running tauri application")
        .run(|app_handle, event| {
            if let RunEvent::Exit = event {
                let state: State<BackendState> = app_handle.state();
                let mut guard = state.0.lock().expect("backend state lock");
                if let Some(child) = guard.as_mut() {
                    let _ = child.kill();
                }
            }
        });
}
