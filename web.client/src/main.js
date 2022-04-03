import Vue from 'vue'
import BootstrapVue from 'bootstrap-vue'
import axios from 'axios'
import router from './router'
import store from './store'
import App from './App'
import PostHub from './post-hub'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import '@fortawesome/fontawesome-free/css/all.css'

Vue.config.productionTip = false
axios.defaults.baseURL = 'https://localhost:44368' // api address
axios.defaults.withCredentials = true

// Include the Authentication header when using JWT authentication
axios.interceptors.request.use(request => {
  if (store.state.context.jwtToken) request.headers['Authorization'] = 'Bearer ' + store.state.context.jwtToken
  return request
})

// Setup axios as the Vue default $http library
Vue.prototype.$http = axios

// Install Vue extensions
Vue.use(BootstrapVue)
Vue.use(PostHub)

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
