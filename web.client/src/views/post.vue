<template>
  <article class="container" v-if="post">
    <header class="row align-items-center">
      <view-count :post="post" class="col-1" />
      <h3 class="col-8">{{ post.title }}</h3>
    </header>
    <p class="row">
      <vue-markdown class="offset-1 col-11">{{ post.body }}</vue-markdown>
    </p>
    <small>by: {{ post.createdBy }}</small>
    <ul v-if="hasComments">
      <li v-for="answer in post.comments" :key="answer.id" class="row border-top py-2">
        <div class="offset-1 col-12">{{ answer.body }}
          <br>
          <small><i>{{ answer.createdBy }}</i></small>
        </div>
      </li>
    </ul>
    <footer>
      <button class="btn btn-primary float-right" v-b-modal.addCommentModal v-if="isAuthenticated"><i class="fas fa-edit"/> Add Comment</button>
      <button class="btn btn-link float-right" @click="onReturnHome">Back to list</button>
    </footer>
    <add-comment :post-id="this.postId" @comment-added="onCommentAdded"/>
  </article>
</template>

<script>
import { mapGetters } from 'vuex'
import VueMarkdown from 'vue-markdown'
import ViewCount from '@/components/view-count'
import AddComment from '@/components/add-comment'

export default {
  components: {
    VueMarkdown,
    ViewCount,
    AddComment
  },
  data () {
    return {
      post: null,
      comments: [],
      postId: this.$route.params.id
    }
  },
  computed: {
    ...mapGetters('context', [
      'isAuthenticated'
    ]),
    hasComments () {
      return this.post.comments.length > 0
    }
  },
  created () {
    this.$http.get(`/api/post/${this.postId}`).then(res => {
      this.post = res.data
      return this.$postHub.postOpened(this.postId)
    })
    this.$postHub.$on('comment-added', this.onCommentAdded)
  },
  beforeDestroy () {
    this.$postHub.postClosed(this.postId)
    this.$postHub.$off('comment-added', this.onCommentAdded)
  },
  methods: {
    onReturnHome () {
      this.$router.push({ name: 'Home' })
    },
    onCommentAdded (comment) {
      if (this.post.id !== comment.postId) return
      if (!this.post.comments.find(a => a.id === comment.id)) {
        this.post.comments.push(comment)
      }
    }
  }
}
</script>
