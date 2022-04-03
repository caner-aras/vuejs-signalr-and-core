<template>
  <h3 class="text-center scoring">
    <button class="btn btn-link btn-lg p-0 d-block mx-auto" :disabled="!isAuthenticated" @click.stop="onUpvote"><i class="fas fa-sort-up" /></button>
    <span class="d-block mx-auto">{{ post.score }}</span>
    <button class="btn btn-link btn-lg p-0 d-block mx-auto" :disabled="!isAuthenticated" @click.stop="onDownvote"><i class="fas fa-sort-down" /></button>
  </h3>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  props: {
    post: {
      type: Object,
      required: true
    }
  },
  computed: {
    ...mapGetters('context', [
      'isAuthenticated'
    ])
  },
  created () {
    this.$postHub.$on('score-changed', this.onScoreChanged)
  },
  beforeDestroy () {
    // cleanup SignalR event
    this.$postHub.$off('score-changed', this.onScoreChanged)
  },
  methods: {
    onUpvote () {
      this.$http.patch(`/api/post/${this.post.id}/upvote`).then(res => {
        Object.assign(this.post, res.data)
      })
    },
    onDownvote () {
      this.$http.patch(`/api/post/${this.post.id}/downvote`).then(res => {
        Object.assign(this.post, res.data)
      })
    },
    // called from .net api through SignalR
    onScoreChanged ({ postId, score }) {
      if (this.post.id !== postId) return
      Object.assign(this.post, { score })
    }
  }
}
</script>

<style scoped>
.scoring .btn-link{
  line-height: 1;
}
</style>
