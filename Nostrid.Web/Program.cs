using Ganss.Xss;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Nostrid;
using Nostrid.Data;
using Nostrid.Interfaces;
using Nostrid.Web;
using Nostrid.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<FeedService>();
builder.Services.AddSingleton<RelayService>();
builder.Services.AddSingleton<EventDatabase>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<HtmlSanitizer>();
builder.Services.AddSingleton<NoteProcessor>();
builder.Services.AddSingleton<Nip05Service>();
builder.Services.AddSingleton<Lud06Service>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<INotificationCounter, NotificationCounter>();
builder.Services.AddSingleton<ConfigService>();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<IClipboardService, ClipboardService>();

var host = builder.Build();

// Database
#if RELEASE
var dbService = host.Services.GetRequiredService<DatabaseService>();
await dbService.InitDatabaseAsync();
var eventDatabase = host.Services.GetRequiredService<EventDatabase>();
eventDatabase.InitDatabase(DatabaseService.FileName);
eventDatabase.DatabaseHasChanged += (_, _) => dbService.StartSyncDatabase();
#else
var eventDatabase = host.Services.GetRequiredService<EventDatabase>();
eventDatabase.InitDatabase(new MemoryStream());
#endif

// Signer
var accountService = host.Services.GetRequiredService<AccountService>();
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
await accountService.AddSigner(new ExtensionSigner(jsRuntime));

await host.RunAsync();
