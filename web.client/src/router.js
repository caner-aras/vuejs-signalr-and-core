import Vue from 'vue'
import Router from 'vue-router'
import HomePage from '@/views/home'
import PostPage from '@/views/post'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Home',
      component: HomePage
    },
    {
      path: '/post/:id',
      name: 'Post',
      component: PostPage
    }
  ]
})
