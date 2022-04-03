<template>
  <b-modal id="addCommentModal" ref="addCommentModal" hide-footer title="Add Comment" @hidden="onHidden">
    <b-form @submit.prevent="onSubmit" @reset.prevent="onCancel">
      <b-form-group label="Your Comment:" label-for="commentInput">
        <b-form-textarea id="commentInput"
                      v-model="form.body"
                      placeholder="Enter a comment"
                      :rows="6"
                      :max-rows="10">
        </b-form-textarea>
      </b-form-group>

      <button class="btn btn-primary float-right ml-2" type="submit" >Publish</button>
      <button class="btn btn-secondary float-right" type="reset">Cancel</button>
    </b-form>
  </b-modal>
</template>

<script>
export default {
  props: {
    postId: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      form: {
        title: '',
        body: ''
      }
    }
  },
  methods: {
    onSubmit (evt) {
      this.$http.post(`api/post/${this.postId}/comments`, this.form).then(res => {
        this.$emit('comment-added', res.data)
        this.$refs.addCommentModal.hide()
      })
    },
    onCancel (evt) {
      this.$refs.addCommentModal.hide()
    },
    onHidden () {
      Object.assign(this.form, {
        title: '',
        body: ''
      })
    }
  }
}
</script>
