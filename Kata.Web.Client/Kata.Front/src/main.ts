import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
// import { ClientGenerated } from './app/services/client-generated';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig);
// .catch((err) =>
//   console.error(err)
// );

// platformBrowserDynamic().bootstrapModule(AppModule);
