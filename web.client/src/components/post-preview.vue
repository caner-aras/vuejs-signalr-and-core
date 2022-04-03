<template>
  <li>
    <div class="card-body row">
      <view-count :post="post" class="col-1" />
      <div class="col-11">
        <div class="card-title row">
          <h5 class="col-8"><a href="#" class="card-link" @click="onOpenPost">{{ post.title }}</a></h5>
        </div>
        <p><vue-markdown :source="post.body" /></p>
        <a href="#" class="card-link" @click="onOpenPost">
          Comment <span class="badge badge-success" v-b-tooltip.d400 title="number of comment(s)">{{ post.answerCount || 0 }}</span>
        </a>
        <br>
        <small>by {{ post.createdBy }} </small>
      </div>
    </div>
    <hr>
  </li>
</template>

<script>
import VueMarkdown from 'vue-markdown'
import ViewCount from '@/components/view-count'

export default {
  components: {
    VueMarkdown,
    ViewCount
  },
  props: {
    post: {
      type: Object,
      required: true
    }
  },
  created () {
    this.$postHub.$on('post-count-changed', this.onCommentCountChanged)
  },
  beforeDestroy () {
    // cleanup SignalR event handlers
    this.$postHub.$off('post-count-changed', this.onCommentCountChanged)
  },
  methods: {
    onOpenPost () {
      this.$router.push({ name: 'Post', params: { id: this.post.id } })
    },
    // called from the .net api through SignalR
    onCommentCountChanged ({ postId, answerCount }) {
      if (this.post.id !== postId) return
      Object.assign(this.post, { answerCount })
    }
  }
}
</script>
