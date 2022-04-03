<template>
  <div id="app">
    <main-navbar />

    <main role="main" class="container mt-4">
      <router-view/>
    </main>

    <login-user />
  </div>
</template>

<script>
import { mapActions } from 'vuex'
import MainNavbar from './components/main-navbar'
import LoginUser from './components/login-user'

export default {
  name: 'App',
  components: {
    MainNavbar,
    LoginUser
  },
  created () {
    this.restoreContext()
  },
  methods: {
    ...mapActions('context', [
      'restoreContext'
    ])
  }
}
</script>
