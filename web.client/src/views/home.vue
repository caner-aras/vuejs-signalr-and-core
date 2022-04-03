<template>
  <div>
    <h1>
      Welcome to my blog
      <button v-b-modal.addPostModal v-if="isAuthenticated" class="btn btn-primary mt-2 float-right">
        <i class="fas fa-plus"/> Add New Post
      </button>
    </h1>
    <ul class="list-unstyled post-previews mt-4">
      <post-preview
        v-for="item in posts"
        :key="item.id"
        :post="item"
        class="mb-3" />
    </ul>
    <add-post @post-added="onPostAdded"/>
  </div>
</template>

<script>
import { mapGetters, mapState } from 'vuex'
import PostPreview from '@/components/post-preview'
import AddPost from '@/components/add-post'

export default {
  components: {
    PostPreview,
    AddPost
  },
  data () {
    return {
      posts: []
    }
  },
  computed: {
    ...mapState('context', [
      'profile'
    ]),
    ...mapGetters('context', [
      'isAuthenticated'
    ])
  },
  created () {
    this.$postHub.$on('post-added', this.onPostAdded)
    this.$http.get('/api/post').then(res => {
      this.posts = res.data
    })
  },
  beforeDestroy () {
    // cleanup SignalR event handlers when removing the component
    this.$postHub.$off('post-added', this.onPostAdded)
  },
  methods: {
    onPostAdded (post) {
      // make sure the post doesnt exist yet (as it will also arrive through signalR!)
      if (this.posts.some(q => q.id === post.id)) return
      this.posts = [post, ...this.posts]
    }
  }
}
</script>
