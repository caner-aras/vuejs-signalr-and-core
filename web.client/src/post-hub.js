import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

export default {
  install (Vue) {
    // use a new Vue instance as the interface for Vue components to receive/send SignalR events
    // this way every component can listen to events or send new events using this.$postHub
    const postHub = new Vue()
    Vue.prototype.$postHub = postHub

    // Provide methods to connect/disconnect from the SignalR hub
    let connection = null
    let startedPromise = null
    let manuallyClosed = false

    Vue.prototype.startSignalR = (jwtToken) => {
      connection = new HubConnectionBuilder()
        .withUrl(
          `${Vue.prototype.$http.defaults.baseURL}/app-hub`,
          jwtToken ? { accessTokenFactory: () => jwtToken } : null
        )
        .configureLogging(LogLevel.Information)
        .build()

      // listen for them in the Vue components
      connection.on('PostAdded', (post) => {
        postHub.$emit('post-added', post)
      })
      connection.on('PostScoreChange', (postId, score) => {
        postHub.$emit('score-changed', { postId, score })
      })
      connection.on('AnswerCountChange', (postId, commentCount) => {
        postHub.$emit('post-count-changed', { postId, commentCount })
      })
      connection.on('CommentAdded', comment => {
        postHub.$emit('comment-added', comment)
      })

      // recommend listening onclose and handling it there.
      function start () {
        startedPromise = connection.start()
          .catch(err => {
            console.error('Failed to connect with hub', err)
            return new Promise((resolve, reject) => setTimeout(() => start().then(resolve).catch(reject), 5000))
          })
        return startedPromise
      }
      connection.onclose(() => {
        if (!manuallyClosed) start()
      })

      // Start everything
      manuallyClosed = false
      start()
    }
    Vue.prototype.stopSignalR = () => {
      if (!startedPromise) return

      manuallyClosed = true
      return startedPromise
        .then(() => connection.stop())
        .then(() => { startedPromise = null })
    }

    postHub.postOpened = (postId) => {
      if (!startedPromise) return

      return startedPromise
        .then(() => connection.invoke('JoinPostGroup', postId))
        .catch(console.error)
    }
    postHub.postClosed = (postId) => {
      if (!startedPromise) return

      return startedPromise
        .then(() => connection.invoke('LeavePostGroup', postId))
        .catch(console.error)
    }
  }
}
