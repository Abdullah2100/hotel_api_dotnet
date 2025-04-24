// store.ts
import { configureStore } from '@reduxjs/toolkit';
import rootReducer from './rootReducer';

// Create the store with the rootReducer using configureStore
const store = configureStore({
  reducer: rootReducer,
});

export default store;
