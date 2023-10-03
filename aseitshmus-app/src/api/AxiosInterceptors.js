import axios from 'axios';

const API_URL = process.env["VUE_APP_BASED_URL"];

const api = axios.create({
  baseURL: API_URL
});

const setAuthToken = () => {
  api.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => {
      return Promise.reject(error);
    }
  );
};

const resetAuthToken = () => {
  api.defaults.headers.common['Authorization'] = '';
}

const setEmailHeader = (emailHeaderValue) => {
 
  api.interceptors.request.use(
    (config) => {
      if (emailHeaderValue) {
        config.headers['Email-Header'] = emailHeaderValue;
      }
      return config;
    },

    (error) => {
      return Promise.reject(error);
    }
  );
};


const interceptors = (logout) => {
  api.interceptors.response.use( 
    async response => {
      return response;
    },
    async error => {
      const originalRequest = error.config;
      if (error.response.status === 401) {
       
        originalRequest._retry = true;
            
        logout();
        document.dispatchEvent(new Event('sessionExpired'));
       }
      return Promise.reject(error);
    }
  );
}


export { api, setAuthToken, resetAuthToken, interceptors, setEmailHeader };
